using MetaExchange.Core;

namespace MetaExchange.Logic
{
    public interface IOutputWriter
    {
        void OutputInitialValues(double balanceEur, double balanceBtc, double amount);

        void OutputResultSequence(IExchangeResult buyResult, IExchangeResult sellResult);

        void OutputString(string toOutput);
    }
}