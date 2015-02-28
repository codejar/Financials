using System.Threading.Tasks;
using Financials.Common.Model;

namespace Financials.Common.Services
{
	public interface ITradeManagementService
	{
		Task<ServerResponse<Trade>> Cancel(int tradeId);
		Task<ServerResponse<Trade>> Execute(int tradeId);
		Task<ServerResponse<Trade>> Create(CreateNewTradeRequest request);
	}
}