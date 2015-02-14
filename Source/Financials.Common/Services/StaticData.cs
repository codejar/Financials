using System;

namespace Financials.Common.Services
{
    public class CurrencyPair
    {
        private readonly string _code;
        private readonly decimal _startingPrice;
        private readonly int _decimalPlaces;
        private readonly int _tickFrequency;
        private readonly int _defaultSpread;
        private readonly decimal _pipSize;

        public CurrencyPair(string code, decimal startingPrice, int decimalPlaces=4, int tickFrequency=10, int defaultSpread=8)
        {
            _code = code;
            _startingPrice = startingPrice;
            _decimalPlaces = decimalPlaces;
            _tickFrequency = tickFrequency;
            _defaultSpread = defaultSpread;
            _pipSize = (decimal)Math.Pow(10, -decimalPlaces);
        }

        public string Code
        {
            get { return _code; }
        }

        public decimal InitialPrice
        {
            get { return _startingPrice; }
        }

        public int DecimalPlaces
        {
            get { return _decimalPlaces; }
        }

        public int TickFrequency
        {
            get { return _tickFrequency; }
        }

        public decimal PipSize
        {
            get { return _pipSize; }
        }

        public int DefaultSpread
        {
            get { return _defaultSpread; }
        }

        #region Equality

        protected bool Equals(CurrencyPair other)
        {
            return string.Equals(_code, other._code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CurrencyPair) obj);
        }

        public override int GetHashCode()
        {
            return (_code != null ? _code.GetHashCode() : 0);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Code: {0}, DecimalPlaces: {1}", _code, _decimalPlaces);
        }
    }

    public class StaticData : IStaticData
    {
        private readonly CurrencyPair[] _currencyPairs =
        {
            new CurrencyPair("GBP/USD",1.6M) ,
            new CurrencyPair("EUR/USD",1.23904M),
            new CurrencyPair("EUR/GBP",0.791339614M),
            new CurrencyPair("NZD/CAD",0.885535855M,8)  ,
            new CurrencyPair("HKD/USD",0.128908M) ,
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