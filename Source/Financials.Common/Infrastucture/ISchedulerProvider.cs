using System.Reactive.Concurrency;

namespace Financials.Common.Infrastucture
{
    public interface ISchedulerProvider
    {
        IScheduler Dispatcher { get; }
        IScheduler TaskPool { get; }
    }
}
