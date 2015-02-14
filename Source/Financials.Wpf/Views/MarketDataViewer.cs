
using System;
using System.Collections.Generic;
using Financials.Common.Model;
using Financials.Common.Services;
using System.Linq;

namespace Financials.Wpf.Views
{

    public class MarketDataStream
    {
        public MarketDataStream(CurrencyPair currencyPair, IObservable<MarketData> marketDataObservable)
        {
        }
    }

    public class MarketDataViewer
    {
        private readonly IStaticData _staticData;
        private readonly IMarketPriceService _marketPriceService;
        private List<IObservable<MarketData>> _prices;

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
