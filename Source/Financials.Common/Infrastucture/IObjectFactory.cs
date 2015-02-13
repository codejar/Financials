namespace Financials.Common.Infrastucture
{
    public interface IObjectProvider
    {
        T Get<T>();
    }
}
