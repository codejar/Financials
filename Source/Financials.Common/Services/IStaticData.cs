namespace Financials.Common.Services
{
    public interface IStaticData
    {
        string[] Customers { get; }
        CurrencyPair[] CurrencyPairs { get; }
    }
}