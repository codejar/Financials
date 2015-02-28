namespace Financials.Common.Infrastucture
{
	public class ServerResponse<T>
		where T :class 
	{

		public ServerResponse(T result)
		{
			Result = result;
		}

		public ServerResponse(string errormessage)
		{
			Error = new ServerError(errormessage);
		}


		public ServerResponse(ServerError error)
		{
			Error = error;
		}

		public ServerError Error { get;private set; }
		public T Result { get; private set; }
	}
}