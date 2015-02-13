using System;
using DynamicData;
using TradeExample.Annotations;
using Financials.Common.Model;

namespace Financials.Common.Services
{
    public interface INearToMarketService
    {
        IObservable<IChangeSet<Trade, long>> Query([NotNull] Func<uint> percentFromMarket);
    }
}