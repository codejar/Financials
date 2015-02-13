using DynamicData;
using Financials.Common.Model;

namespace Financials.Common.Services
{
    public interface ITradeService
    {
        IObservableCache<Trade, long> Trades { get; }
    }
}