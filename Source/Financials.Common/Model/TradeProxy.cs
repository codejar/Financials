using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Financials.Common.Infrastucture;

namespace Financials.Common.Model
{
    public class TradeProxy:NotifyPropertyChangedBase, IDisposable, IEquatable<TradeProxy>
    {
        private readonly Trade _trade;
        private readonly IDisposable _cleanUp;
        private bool _recent;
        private readonly long _id;

        public TradeProxy(Trade trade)
        {
            _id = trade.Id;
            _trade = trade;

            var isRecent = DateTime.Now.Subtract(trade.Timestamp).TotalSeconds < 2;
            var recentIndicator= Disposable.Empty;
                         
            if (isRecent)
            {
                Recent = true;
                recentIndicator = Observable.Timer(TimeSpan.FromSeconds(2))
                    .Subscribe(_ => Recent = false);
            }


            //market price changed is an observable on the trade object
            var priceRefresher = trade.MarketPriceChanged
                .Subscribe(_ =>
                           {
                               OnPropertyChanged("MarketPrice");
                               OnPropertyChanged("PercentFromMarket");
                           });

            _cleanUp = new CompositeDisposable(recentIndicator, priceRefresher);
        }

        public decimal MarketPrice
        {
            get { return _trade.MarketPrice; }
        }

        public decimal PercentFromMarket
        {
            get { return _trade.PercentFromMarket; }
        }

        public bool Recent
        {
            get { return _recent; }
            set
            {
                if (_recent==value) return;
                _recent = value;
                OnPropertyChanged();
            }
        }

        public Trade Trade
        {
            get { return _trade; }
        }

        #region Equaility Members

        public bool Equals(TradeProxy other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _id == other._id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TradeProxy) obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(TradeProxy left, TradeProxy right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TradeProxy left, TradeProxy right)
        {
            return !Equals(left, right);
        }

        #endregion

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}