
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Financials.Common.Services;
using System.Linq;
using Financials.Common.Infrastucture;

namespace Financials.Wpf.Views
{
    public class MarketDataViewer: IDisposable
    {
	    private readonly IDisposable _cleanUp;

		public IEnumerable<MarketDataTicker> Prices { get; }

		public MarketDataViewer(IStaticData staticData, IMarketDataService marketDataService)
        {

            Prices = staticData.CurrencyPairs.Select(currencypair =>
            {
                var observable = marketDataService.Watch(currencypair.Code);
                return new MarketDataTicker(currencypair, observable);

            }).ToList();

            _cleanUp = Disposable.Create(() =>
            {
                Prices.OfType<IDisposable>().ForEach(d=>d.Dispose());
            });

        }


	    public void Dispose()
        {
          _cleanUp.Dispose();
        }
    }
}
