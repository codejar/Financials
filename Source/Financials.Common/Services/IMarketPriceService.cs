using System;
using Financials.Common.Model;

namespace Financials.Common.Services
{


    public interface IMarketPriceService
    {
        IObservable<MarketData> ObservePrice(string currencyPair);
    }
    
}