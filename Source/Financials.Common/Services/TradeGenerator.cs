using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using DynamicData.Kernel;
using Financials.Common.Infrastucture;
using Financials.Common.Model;

namespace Financials.Common.Services
{
	public interface ITradeGenerator : IDisposable
	{
		IEnumerable<Trade> Generate(int numberToGenerate, bool initialLoad = false);
		
		/// <summary>
		/// Generates the next available trade id.
		/// </summary>
		/// <returns></returns>
		int NextId();
	}

	public class TradeGenerator: ITradeGenerator
	{
        private readonly Random _random= new Random();
        private readonly IStaticData _staticData;
        private readonly IDisposable _cleanUp;
        private readonly IDictionary<string, MarketData> _latestPrices = new Dictionary<string, MarketData>();
        private readonly object _locker = new object();
        private int _counter = 0;

        public TradeGenerator(IStaticData staticData, IMarketDataService marketDataService)
        {
            _staticData = staticData;

            //keep track of the latest price so we can generate trades are a reasonable distance from the market
            _cleanUp = staticData.CurrencyPairs
                                    .Select(currencypair => marketDataService.Watch(currencypair.Code)).Merge()
                                    .Synchronize(_locker)
                                    .Subscribe(md =>
                                    {
                                        _latestPrices[md.Instrument] = md;
                                    });

        }


	    public int NextId()
	    {
			return Interlocked.Increment(ref _counter);
	    }

		public IEnumerable<Trade> Generate(int numberToGenerate, bool initialLoad = false)
        {
            Func<Trade> newTrade = () =>
            {
                var id = NextId();
                var bank = _staticData.Customers[_random.Next(0, _staticData.Customers.Length)];
                var pair = _staticData.CurrencyPairs[_random.Next(0, _staticData.CurrencyPairs.Length)];
                var amount = (_random.Next(1, 2000) / 2) * (10 ^ _random.Next(1, 5));
                var buySell = _random.NextBoolean() ? BuyOrSell.Buy : BuyOrSell.Sell;
                
                if (initialLoad)
                {
                    var status = _random.NextDouble() > 0.5 ? TradeStatus.Live : TradeStatus.Cancelled;
                    var seconds = _random.Next(1, 60 * 60 * 24);
                    var time = DateTime.Now.AddSeconds(-seconds);
                    return new Trade(id, bank, pair.Code, status, buySell, GererateRandomPrice(pair, buySell), amount, timeStamp: time);
                }
                return new Trade(id, bank, pair.Code, TradeStatus.Live, buySell, GererateRandomPrice(pair, buySell), amount);
            };


            IEnumerable<Trade> result;
            lock (_locker)
            {
                result = Enumerable.Range(1, numberToGenerate).Select(_ => newTrade()).ToArray();
            }
            return result;
        }


        private decimal GererateRandomPrice(CurrencyPair currencyPair,BuyOrSell buyOrSell)
        {

            var price = _latestPrices.Lookup(currencyPair.Code)
                                .ConvertOr(md=>md.Bid, () => currencyPair.InitialPrice);

            //generate percent price 1-100 pips away from the market
            var pipsFromMarket = _random.Next(1, 100);
            var adjustment = Math.Round(pipsFromMarket * currencyPair.PipSize, currencyPair.DecimalPlaces);
            return buyOrSell==BuyOrSell.Sell ? price + adjustment : price - adjustment;
        }



		public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}