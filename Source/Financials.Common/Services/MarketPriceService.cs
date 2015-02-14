using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData.Kernel;
using Financials.Common.Model;


namespace Financials.Common.Services
{
    
    public  class MarketPriceService : IMarketPriceService
    {
        private readonly Dictionary<string, IObservable<MarketData>> _prices = new Dictionary<string, IObservable<MarketData>>();
        
        public MarketPriceService(IStaticData staticData)
        {
            //generate hot observable for all currency pairs
            foreach (var item in staticData.CurrencyPairs)
            {
                _prices[item.Code] = GenerateStream(item);
            }
        }

        private IObservable<MarketData> GenerateStream(CurrencyPair currencyPair)
        {

            return Observable.Create<MarketData>(observer =>
            {
                        var spread = currencyPair.DefaultSpread;
                        var midRate = currencyPair.InitialPrice;
                        var bid = midRate - (spread * currencyPair.PipSize);
                        var offer = midRate + (spread * currencyPair.PipSize);
                        var initial = new MarketData(currencyPair.Code, bid, offer);
                
                        var currentPrice = initial;
                        
                        observer.OnNext(initial);

                        var random = new Random();
                        var period = random.Next(250, 1500);

                        //for a given period, move prices by up to 5 pips
                        return Observable.Interval(TimeSpan.FromMilliseconds(period))
                            .Select(_ =>  random.Next(1, 5))
                            .Subscribe(pips =>
                            {
                                //move up or down between 1 and 5 pips
                                currentPrice = random.NextDouble() > 0.5 ? 
                                                currentPrice + (pips * currencyPair.PipSize):
                                                currentPrice - (pips * currencyPair.PipSize);

                                observer.OnNext(currentPrice);

                            });
            });
        }

        public IObservable<MarketData> ObservePrice(string currencyPair)
        {
            return _prices.Lookup(currencyPair)
                .ValueOrThrow(() => new Exception(currencyPair + " is an unknown currency pair"));
        }
    }
}