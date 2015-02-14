using System;
using Financials.Common.Model;

namespace Financials.Common.Services
{
   

    public class StaticData : IStaticData
    {
        private readonly CurrencyPair[] _currencyPairs =
        {
            new CurrencyPair("GBP/USD",1.6M) ,
            new CurrencyPair("EUR/USD",1.23904M),
            new CurrencyPair("EUR/GBP",0.791339614M),
            new CurrencyPair("NZD/CAD",0.885535855M,8)  ,
            new CurrencyPair("HKD/USD",0.128908M,6) ,
            new CurrencyPair("NOK/SEK",1.10M) ,
            new CurrencyPair("XAU/GBP",768.399M,5) ,
            new CurrencyPair("USD/JPY",118.81M,2),
        };

        private readonly string[] _customers = new[] { "Bank of America", "Bank of Europe", "Bank of England","BNP Paribas" };


        public string[] Customers
        {
            get { return _customers; }
        }

        public CurrencyPair[] CurrencyPairs
        {
            get { return _currencyPairs; }
        }
    }
}