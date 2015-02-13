﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using DynamicData.Operators;
using TradeExample.Annotations;
using TradeExample.Infrastucture;

namespace TradeExample
{
    public enum TimePeriod
    {
        LastMinute,
        LastHour,
        Older
    }

    public class TradesByTime : IDisposable, IEquatable<TradesByTime>
    {
        private readonly IObservableCollection<TradeProxy> _data;
        private readonly IDisposable _cleanUp;
        private readonly TimePeriod _period;

        public TradesByTime([NotNull] IGroup<Trade, long, TimePeriod> @group,
            ISchedulerProvider schedulerProvider)
        {
            if (@group == null) throw new ArgumentNullException("group");
            _period = @group.Key;

            _data = new ObservableCollectionExtended<TradeProxy>();

            _cleanUp = @group.Cache.Connect()
                        .Transform(trade => new TradeProxy(trade))
                        .Sort(SortExpressionComparer<TradeProxy>.Descending(p => p.Timestamp), SortOptimisations.ComparesImmutableValuesOnly)
                        .ObserveOn(schedulerProvider.Dispatcher)
                        .Bind(_data)
                        .DisposeMany()
                        .Subscribe();
        }

        #region Equality

        public bool Equals(TradesByTime other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _period == other._period;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TradesByTime) obj);
        }

        public override int GetHashCode()
        {
            return (int) _period;
        }

        public static bool operator ==(TradesByTime left, TradesByTime right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TradesByTime left, TradesByTime right)
        {
            return !Equals(left, right);
        }

        #endregion

        public TimePeriod Period
        {
            get { return _period; }
        }

        public string Description
        {
            get
            {
                switch (Period)
                {
                    case TimePeriod.LastMinute:
                        return "Last Minute";
                    case TimePeriod.LastHour:
                         return "Last Hour";;
                    case TimePeriod.Older:
                        return "Old";
                    default:
                        return "Unknown";
                }
            }
        }

        public IObservableCollection<TradeProxy> Data
        {
            get { return _data; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

    public class TradesByTimeViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly IDisposable _cleanUp;
        private readonly IObservableCollection<TradesByTime> _data = new ObservableCollectionExtended<TradesByTime>();

        public TradesByTimeViewer(ITradeService tradeService, ISchedulerProvider schedulerProvider)
        {
            _schedulerProvider = schedulerProvider;

            var groupController = new GroupController();

            var grouperRefresher = Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ => groupController.RefreshGroup());

            var loader = tradeService.Trades.Connect()
                .Group(trade =>
                       {
                           var diff = DateTime.Now.Subtract(trade.Timestamp);
                           if (diff.TotalSeconds <= 60) return TimePeriod.LastMinute;
                           if (diff.TotalMinutes <= 60) return TimePeriod.LastHour;
                           return TimePeriod.Older;
                       }, groupController)
                .Transform(group => new TradesByTime(group, _schedulerProvider))
                .Sort(SortExpressionComparer<TradesByTime>.Ascending(t => t.Period))
                .ObserveOn(_schedulerProvider.Dispatcher)
                .Bind(_data)
                .DisposeMany()
                .Subscribe();
            
            _cleanUp = new CompositeDisposable(loader, grouperRefresher);
        }


        public IObservableCollection<TradesByTime> Data
        {
            get { return _data; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}