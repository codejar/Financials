using System;
using Financials.Common.Infrastucture;
using Financials.Common.Model;

namespace Financials.Wpf.Views
{
    public class MarketDataTicker : NotifyPropertyChangedBase, IDisposable
    {
        private readonly CurrencyPair _currencyPair;
        private readonly IDisposable _cleanUp;
        private decimal _bid;
        private decimal _offer;
        private Trend _trend;

        public MarketDataTicker(CurrencyPair currencyPair, IObservable<MarketData> marketDataObservable)
        {
            if (currencyPair == null) throw new ArgumentNullException("currencyPair");
            if (marketDataObservable == null) throw new ArgumentNullException("marketDataObservable");
            _currencyPair = currencyPair;

            _cleanUp = marketDataObservable.
                Subscribe(md =>
                {
                    Trend = Bid > md.Bid ? Trend.Up : Trend.Down;
                    Bid = md.Bid;
                    Offer = md.Offer;
                });
        }

        public CurrencyPair CurrencyPair
        {
            get { return _currencyPair; }
        }

        public Trend Trend
        {
            get { return _trend; }
            set { SetAndRaise(ref _trend, value); }
        }

        public decimal Bid
        {
            get { return _bid; }
            set { SetAndRaise(ref _bid, value); }
        }

        public decimal Offer
        {
            get { return _offer; }
            set { SetAndRaise(ref _offer, value); }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}