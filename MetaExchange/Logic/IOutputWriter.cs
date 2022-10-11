using MetaExchange.Core;

namespace MetaExchange.Logic
{
    public interface IOutputWriter
    {
        void OutputResultSequence(IExchangeResult result);

        void OutputString(string toOutput);
    }
}