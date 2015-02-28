using System;
using Financials.Common.Model;

namespace Financials.Common.Infrastucture
{
	public class CreateNewTradeRequest
	{
		public BuyOrSell BuyOrSell { get; }

		public CurrencyPair CurrencyPair { get; }
		public int Amount { get; }

		public decimal Rate { get; }

		public CreateNewTradeRequest(BuyOrSell buyOrSell,CurrencyPair currencyPair,int amount, decimal rate)
		{
			if (currencyPair == null) throw new ArgumentNullException("currencyPair");
			BuyOrSell = buyOrSell;
			CurrencyPair = currencyPair;
			Amount = amount;
			Rate = rate;
		}

	}
}