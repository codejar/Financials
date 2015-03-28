using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Financials.Common.Infrastucture
{
	public interface IMessagePublisher
	{
		void Publish<T>(T message);
	}

	public interface IMessageListener
	{
		IObservable<T> Listen<T>();
	}


	public class MessageBroker<T> : IMessagePublisher<T>, IMessageListener<T>
	{
		private readonly ISubject<T> _messages = new Subject<T>();

		public void Publish(T message)
		{
			if (message == null) throw new ArgumentNullException("message");
			_messages.OnNext(message);
		}

		public IObservable<T> Messages => _messages.AsObservable();
	}



}
