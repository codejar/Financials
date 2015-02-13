using System;

namespace Financials.Common.Services
{
    public interface IMarketPriceService
    {
        IObservable<decimal> ObservePrice(string currencyPair);
    }
}