﻿using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using DynamicData.Kernel;
using DynamicData.Operators;
using DynamicData.PLinq;
using Financials.Common.Infrastucture;
using Financials.Common.Model;
using Financials.Common.Services;

namespace Financials.Wpf.Views
{
    public class LiveTradesViewer :NotifyPropertyChangedBase, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IDisposable _cleanUp;
        private readonly IObservableCollection<TradeProxy> _data = new ObservableCollectionExtended<TradeProxy>();
        private readonly FilterController<Trade> _filter = new FilterController<Trade>();
        private string _searchText;

        public LiveTradesViewer(ILogger logger,ITradeService tradeService, ISchedulerProvider schedulerProvider)
        {
            _logger = logger;

            var filterApplier = this.ObserveChanges()
                                .Throttle(TimeSpan.FromMilliseconds(250)).ToUnit()   
                                .StartWith(Unit.Default)
                                .Subscribe(_ => ApplyFilter());

            var loader = tradeService.Trades.Connect(trade => trade.Status == TradeStatus.Live) //prefilter live trades only
                .Filter(_filter) // apply user filter
                .Transform(trade => new TradeProxy(trade),new ParallelisationOptions(ParallelType.Ordered,5))
                .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp),SortOptimisations.ComparesImmutableValuesOnly)
                .ObserveOn(schedulerProvider.Dispatcher)
                .Bind(_data)    // update observable collection bindings
                .DisposeMany()  //since TradeProxy is disposable dispose when no longer required
                .Subscribe();

            _cleanUp = new CompositeDisposable(loader, _filter, filterApplier);
        }

        private void ApplyFilter()
        {
            _logger.Info("Applying filter");
            if (string.IsNullOrEmpty(SearchText))
            {
                _filter.ChangeToIncludeAll();
            }
            else
            {
                _filter.Change(t => t.CurrencyPair.Contains(SearchText,StringComparison.OrdinalIgnoreCase) ||
                                    t.Customer.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
            _logger.Info("Applied filter");
        }

        public string SearchText
        {
            get { return _searchText; }
            set  { SetAndRaise(ref _searchText,value);}
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
}