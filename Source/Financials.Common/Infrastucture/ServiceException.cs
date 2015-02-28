using System;

namespace Financials.Common.Infrastucture
{
	public class ServiceException : Exception
	{
		public ServiceException(string message) : base(message)
		{
		}

		public ServiceException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}