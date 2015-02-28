using System;
using System.Linq;
using System.Reactive.Disposables;
using DynamicData.Kernel;
using Financials.Common.Infrastucture;
using Financials.Common.Model;

namespace Financials.Common.Services
{
	/// <summary>
	/// Job to generate random trades, starting with an inital batch.
	/// 
	/// Used to simulate a rapaidly moving data
	/// </summary>
	public class TradeGeneratorJob : IDisposable
	{
		private readonly IMessagePublisher<Trade> _publisher;
		private readonly ITradeGenerator _tradeGenerator;
		private readonly  Lazy<ITradesCache>  _tradeService;
		private readonly ISchedulerProvider _schedulerProvider;
		private readonly ILogger _logger;
		private readonly IDisposable _runner;

		public TradeGeneratorJob(IMessagePublisher<Trade> publisher,
			ITradeGenerator tradeGenerator, 
			Lazy<ITradesCache> tradeService,
			ISchedulerProvider schedulerProvider,
			ILogger logger)
		{
			_publisher = publisher;
			_tradeGenerator = tradeGenerator;
			_tradeService = tradeService;
			_schedulerProvider = schedulerProvider;
			_logger = logger;

			_runner = GenerateTradesAndMaintainCache();
		}

		private IDisposable GenerateTradesAndMaintainCache()
		{
			//bit of code to generate trades
			var random = new Random();

			//initally load some trades 
			var initial = _tradeGenerator.Generate(5000, true);
			initial.ForEach(_publisher.Publish);

			Func<TimeSpan> randomInterval = () =>
			{
				var ms = random.Next(1000, 10000);
				return TimeSpan.FromMilliseconds(ms);
			};

			// create a random number of new trades at a random interval
			var tradeGenerator = _schedulerProvider.TaskPool
				.ScheduleRecurringAction(randomInterval, () =>
				{
					var number = random.Next(1, 7);
					_logger.Info("Adding {0} trades", number);
					var trades = _tradeGenerator.Generate(number);

					trades.ForEach(_publisher.Publish);
				});

			// close a random number of trades at a random interval
			var tradeCloser = _schedulerProvider.TaskPool
				.ScheduleRecurringAction(randomInterval, () =>
				{

					var number = random.Next(1, 8);
					_logger.Info("Closing {0} trades", number);
					var trades = _tradeService.Value.Trades.Items
						.Where(trade => trade.Status == TradeStatus.Live)
						.OrderBy(t => Guid.NewGuid()).Take(number).ToArray();

					var toClose = trades.Select(trade => new Trade(trade, TradeStatus.Cancelled));
					toClose.ForEach(_publisher.Publish);
					_logger.Info("Cancelled {0} trades", number);
				});

			return new CompositeDisposable(tradeGenerator, tradeCloser);

		}

		public void Dispose()
		{
			_runner.Dispose();
		}
	}
}