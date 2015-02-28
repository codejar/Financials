using System;

namespace Financials.Common.Infrastucture
{
	public interface IMessageListener<out T>
	{
		IObservable<T> Messages { get; }
	}
}