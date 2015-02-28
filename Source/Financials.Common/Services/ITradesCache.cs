using DynamicData;
using Financials.Common.Model;

namespace Financials.Common.Services
{
    public interface ITradesCache
    {
        IObservableCache<Trade, long> Trades { get; }
    }
}