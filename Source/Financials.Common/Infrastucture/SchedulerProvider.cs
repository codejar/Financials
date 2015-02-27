using System.Reactive.Concurrency;
using System.Windows.Threading;

namespace Financials.Common.Infrastucture
{
    public class SchedulerProvider : ISchedulerProvider
    {
	    public SchedulerProvider(Dispatcher dispatcher)
        {
            Dispatcher = new DispatcherScheduler(dispatcher);
        }

        public IScheduler Dispatcher { get; }

	    public IScheduler TaskPool => TaskPoolScheduler.Default;
    }
}