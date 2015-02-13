
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Financials.Common.Services;
using System.Linq;

namespace Financials.Wpf.Views
{
    public class MarketDataViewer
    {
        private readonly IStaticData _staticData;
        private readonly IMarketPriceService _marketPriceService;
        private List<IObservable<decimal>> _prices;

        public MarketDataViewer(IStaticData staticData, IMarketPriceService marketPriceService)
        {
            _staticData = staticData;
            _marketPriceService = marketPriceService;

            _prices = staticData.CurrencyPairs.Select(currencypair => marketPriceService.ObservePrice(currencypair.Code)).ToList();



        }

        public CurrencyPair[] CurrencyPairs
        {
            get { return _staticData.CurrencyPairs; }
        }

    }
}
