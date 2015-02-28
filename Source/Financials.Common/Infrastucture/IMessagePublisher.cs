namespace Financials.Common.Infrastucture
{
	public interface IMessagePublisher<in T>
	{
		void Publish(T message);
	}
}