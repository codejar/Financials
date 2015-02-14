using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData.Kernel;
using Financials.Common.Model;

namespace Financials.Common.Services
{
    public class TradeGenerator: IDisposable
    {
        private readonly Random _random= new Random();
        private readonly IStaticData _staticData;
        private readonly IMarketDataService _marketDataService;
        private readonly IDisposable _cleanUp;
        private readonly IDictionary<string, MarketData> _latestPrices = new Dictionary<string, MarketData>();
        private readonly object _locker = new object();
        private int _counter = 0;

        public TradeGenerator(IStaticData staticData, IMarketDataService marketDataService)
        {
            _staticData = staticData;
            _marketDataService = marketDataService;

            //keep track of the latest price so we can generate trades are a reasonable distance from the market
            _cleanUp = staticData.CurrencyPairs
                                    .Select(currencypair => marketDataService.Watch(currencypair.Code)).Merge()
                                    .Synchronize(_locker)
                                    .Subscribe(md =>
                                    {
                                        _latestPrices[md.Instrument] = md;
                                    });

        }

        public IEnumerable<Trade> Generate(int numberToGenerate, bool initialLoad = false)
        {
            Func<Trade> newTrade = () =>
            {
                var id = _counter++;
                var bank = _staticData.Customers[_random.Next(0, _staticData.Customers.Length)];
                var pair = _staticData.CurrencyPairs[_random.Next(0, _staticData.CurrencyPairs.Length)];
                var amount = (_random.Next(1, 2000) / 2) * (10 ^ _random.Next(1, 5));
                if (initialLoad)
                {
                    var status = _random.NextDouble() > 0.5 ? TradeStatus.Live : TradeStatus.Closed;
                    var seconds = _random.Next(1, 60 * 60 * 24);
                    var time = DateTime.Now.AddSeconds(-seconds);
                    return new Trade(id, bank, pair.Code, status, GererateRandomPrice(pair), amount, timeStamp: time);
                }
                return new Trade(id, bank, pair.Code, TradeStatus.Live, GererateRandomPrice(pair), amount);
            };


            IEnumerable<Trade> result;
            lock (_locker)
            {
                result = Enumerable.Range(1, numberToGenerate).Select(_ => newTrade()).ToArray();
            }
            return result;
        }


        private decimal GererateRandomPrice(CurrencyPair currencyPair)
        {

            var price = _latestPrices.Lookup(currencyPair.Code)
                                .ConvertOr(md=>md.Bid, () => currencyPair.InitialPrice);

            //generate percent price 1-10% away from the inital market
            var pcFromMarket = _random.Next(1, 1000)/(decimal) 5000;

            var positive = _random.NextDouble() > 0.5;

            return positive
                ? price + (pcFromMarket * price)
                : price - (pcFromMarket * price);
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}