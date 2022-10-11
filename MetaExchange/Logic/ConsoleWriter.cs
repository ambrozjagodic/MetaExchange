using MetaExchange.Core;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace MetaExchange.Logic
{
    [ExcludeFromCodeCoverage]
    public class ConsoleWriter : IOutputWriter
    {
        public void OutputResultSequence(IExchangeResult result)
        {
            Console.WriteLine("Results:");
            Console.WriteLine();
            string buyResultStr = JsonConvert.SerializeObject(result);
            Console.WriteLine(buyResultStr);
            Console.WriteLine();
            Console.WriteLine();
        }

        public void OutputString(string toOutput)
        {
            Console.WriteLine(toOutput);
        }
    }
}