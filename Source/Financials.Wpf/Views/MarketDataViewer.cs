
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Financials.Common.Services;
using System.Linq;

namespace Financials.Wpf.Views
{
    public class MarketDataViewer: IDisposable
    {
        private readonly IEnumerable<MarketDataTicker> _prices;
      
        private readonly IDisposable _cleanUp;

        public MarketDataViewer(IStaticData staticData, IMarketPriceService marketPriceService)
        {

            _prices = staticData.CurrencyPairs.Select(currencypair =>
            {
                var observable = marketPriceService.ObservePrice(currencypair.Code);
                return new MarketDataTicker(currencypair, observable);

            }).ToList();

            _cleanUp = Disposable.Create(() =>
            {
                _prices.OfType<IDisposable>().ForEach(d=>d.Dispose());
            });

        }

        public IEnumerable<MarketDataTicker> Prices
        {
            get { return _prices; }
        }


        public void Dispose()
        {
          _cleanUp.Dispose();
        }
    }
}
