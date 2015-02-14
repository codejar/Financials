using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Financials.Wpf.Views;
using Financials.Common.Infrastucture;

namespace Financials.Wpf.Infrastucture
{
    public class MenuItems: IDisposable
    {
        private readonly ILogger _logger;
        private readonly IObjectProvider _objectProvider;
        private readonly IEnumerable<MenuItem> _menu;
        private readonly ISubject<ViewContainer> _viewCreatedSubject = new Subject<ViewContainer>();
        private readonly IDisposable _cleanUp;

        public MenuItems(ILogger logger, IObjectProvider objectProvider)
        {
            _logger = logger;
            _objectProvider = objectProvider;

            _menu = new List<MenuItem>
            {
                new MenuItem("Live Trades",
                    "...crap...",
                    () => Open<LiveTradesViewer>("Live Trades")),
                
                new MenuItem("Market Data",
                "...",
                () => Open<MarketDataViewer>("Market Data Viewer")),

            };
            
            _cleanUp = Disposable.Create(() => _viewCreatedSubject.OnCompleted());
        }

        private async void Open<T>(string title)
        {
            var content = await RunAsync<T>(title); _objectProvider.Get<T>();
            _logger.Info("--Resolved '{0}'", title);
            _viewCreatedSubject.OnNext(new ViewContainer(title, content));

        }

         private async Task<T> RunAsync<T>(string title)
        {
            _logger.Info("Opening '{0}'", title);

             return await Task.Factory.StartNew(() =>
             {
                 _logger.Info("Resolving '{0}'", title);
                 return _objectProvider.Get<T>();
             });
        }



        public IObservable<ViewContainer> ItemCreated
        {
            get { return _viewCreatedSubject.AsObservable(); }
        }

        public IEnumerable<MenuItem> Menu
        {
            get { return _menu; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}