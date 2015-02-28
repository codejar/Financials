using System;
using System.Threading;
using System.Threading.Tasks;
using Financials.Common.Infrastucture;
using Financials.Common.Model;

namespace Financials.Common.Services
{
	/// <summary>
	/// Ficticious trade management service. This is the hook where a real world code example would go to a server / web api.
	/// But in this demo, we are not trying to show you beautiful people how to speak to a server. 
	/// Such things are standard and loads of examples elsewhere!
	/// </summary>
	public class TradeManagementService : ITradeManagementService
	{
		private readonly ITradeService _tradeService;
		private readonly ITradeGenerator _tradeGenerator;
		private readonly IMessagePublisher<Trade> _messagePublisher;

		public TradeManagementService(ITradeService tradeService,
			ITradeGenerator tradeGenerator,
			IMessagePublisher<Trade> messagePublisher )
		{
			_tradeService = tradeService;
			_tradeGenerator = tradeGenerator;
			_messagePublisher = messagePublisher;
		}

		public async Task<ServerResponse<Trade>> Cancel(int tradeId)
		{
			return await Task.Factory.StartNew(() =>
			{
				//simulate latency 
				Thread.Sleep(TimeSpan.FromMilliseconds(250));

				var trade = _tradeService.Trades.Lookup(tradeId);
				if (!trade.HasValue)
					return new ServerResponse<Trade>(string.Format("Cannot cancel. Trade {0} does not exist", tradeId));

				var cancelledTrade = new Trade(trade.Value, TradeStatus.Cancelled);
				_messagePublisher.Publish(cancelledTrade);
				return new ServerResponse<Trade>(cancelledTrade);

			});
		}
		
		public async Task<ServerResponse<Trade>> Execute(int tradeId)
		{
			return await Task.Factory.StartNew(() =>
			{
				//simulate latency 
				Thread.Sleep(TimeSpan.FromMilliseconds(250));

				var trade = _tradeService.Trades.Lookup(tradeId);
				if (!trade.HasValue)
					return new ServerResponse<Trade>(string.Format("Cannot execute. Trade {0} does not exist", tradeId));

				var cancelledTrade = new Trade(trade.Value, TradeStatus.Executed);
				_messagePublisher.Publish(cancelledTrade);
				return new ServerResponse<Trade>(cancelledTrade);
			});
		}

		public async Task<ServerResponse<Trade>> Create(CreateNewTradeRequest request)
		{
			if (request == null) throw new ArgumentNullException("request");
			return await Task.Factory.StartNew(() =>
			{
				//simulate latency 
				Thread.Sleep(TimeSpan.FromMilliseconds(250));
				var nextId = _tradeGenerator.NextId();

				var newTrade = new Trade(nextId, "MyCustomer", 
					request.CurrencyPair.Code, 
					TradeStatus.Live, 
					request.BuyOrSell,
					request.Rate, request.Amount);

				_messagePublisher.Publish(newTrade);
				return new ServerResponse<Trade>(new ServerError("Not implemented"));
			});
		}
	}
}