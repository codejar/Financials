using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Financials.Common.Infrastucture;
using Financials.Common.Model;

namespace Financials.Common.Services
{
	public class TradeService : ITradeService, IDisposable
    {
        private readonly ILogger _logger;

		private readonly ISourceCache<Trade, long> _tradesSource = new SourceCache<Trade, long>(trade => trade.Id);
        private readonly IObservableCache<Trade, long> _tradesCache;
        private readonly IDisposable _cleanup;
		private int _nextId;

        public TradeService(ILogger logger,IMessageListener<Trade> pseudoServiceMessages)
        {
	        if (logger == null) throw new ArgumentNullException("logger");
	        if (pseudoServiceMessages == null) throw new ArgumentNullException("pseudoServiceMessages");

	        _logger = logger;

	        //call AsObservableCache() to hide the update methods as we are exposing the cache
            _tradesCache = _tradesSource.AsObservableCache();

			//get any stuff from the trade pretend service
	        pseudoServiceMessages.Messages
				.Buffer(TimeSpan.FromMilliseconds(125))
				.Where(buffer=> buffer.Count!=0)
				.Subscribe(changes => _tradesSource.AddOrUpdate(changes));

            _cleanup = new CompositeDisposable(_tradesCache, _tradesSource);
        }

        public IObservableCache<Trade, long> Trades => _tradesCache;
		public int NextId()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
        {
            _cleanup.Dispose();
        }
    }
}