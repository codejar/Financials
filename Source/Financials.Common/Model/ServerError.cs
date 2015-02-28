namespace Financials.Common.Model
{
	public class ServerError
	{
		public ServerError(string message)
		{
			Message = message;
		}

		public string Message { get; }
	}
}