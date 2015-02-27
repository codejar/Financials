using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using DynamicData.Kernel;
using DynamicData.Operators;
using DynamicData.PLinq;
using Financials.Common.Infrastucture;
using Financials.Common.Model;
using Financials.Common.Services;

namespace Financials.Wpf.Views
{
    public class LiveTradesViewer :NotifyPropertyChangedBase, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IDisposable _cleanUp;
        private readonly IObservableCollection<TradeProxy> _data = new ObservableCollectionExtended<TradeProxy>();
        private readonly FilterController<Trade> _filter = new FilterController<Trade>();
        private string _searchText;

        public LiveTradesViewer(ILogger logger,ITradeService tradeService, ISchedulerProvider schedulerProvider)
        {
            _logger = logger;

			var filterApplier = this.PropertyValueChanged(t => t.SearchText)
				.Throttle(TimeSpan.FromMilliseconds(250))
				.Select(propargs => BuildFilter(propargs.Value))
				.Subscribe(_filter.Change);

			var loader = tradeService.Trades.Connect(trade => trade.Status == TradeStatus.Live) //prefilter live trades only
                .Filter(_filter) // apply user filter
                .Transform(trade => new TradeProxy(trade),new ParallelisationOptions(ParallelType.Ordered,5))
                .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Trade.Timestamp),SortOptimisations.ComparesImmutableValuesOnly)
                .ObserveOn(schedulerProvider.Dispatcher)
                .Bind(_data)    // update observable collection bindings
                .DisposeMany()  //since TradeProxy is disposable dispose when no longer required
                .Subscribe();

            _cleanUp = new CompositeDisposable(loader, _filter, filterApplier);
        }

		private Func<Trade, bool> BuildFilter(string searchText)
		{
			_logger.Info("Building filter for {0}", searchText);
			if (string.IsNullOrEmpty(searchText))
				return trade => true;

			return t => t.CurrencyPair.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
											t.Customer.Contains(searchText, StringComparison.OrdinalIgnoreCase);
		}

        public string SearchText
        {
            get { return _searchText; }
            set  { SetAndRaise(ref _searchText,value);}
        }

        public IObservableCollection<TradeProxy> Data => _data;

	    public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}