using System;
using Financials.Common.Model;

namespace Financials.Common.Services
{
    public interface IMarketDataService
    {
        IObservable<MarketData> Watch(string currencyPair);
    }
    
}