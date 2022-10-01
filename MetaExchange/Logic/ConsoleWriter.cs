using MetaExchange.Core;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace MetaExchange.Logic
{
    [ExcludeFromCodeCoverage]
    public class ConsoleWriter : IOutputWriter
    {
        public void OutputInitialValues(double balanceEur, double balanceBtc, double amount)
        {
            Console.WriteLine("###############################################################################################################");
            Console.WriteLine("###############################################################################################################");
            Console.WriteLine("###############################################################################################################");
            Console.WriteLine("###############################################################################################################");
            Console.WriteLine("###############################################################################################################");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"Processing input: \n\tBalanceEur={balanceEur};\n\tBalanceBTC={balanceBtc};\n\tAmount={amount};");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        public void OutputResultSequence(IExchangeResult buyResult, IExchangeResult sellResult)
        {
            Console.WriteLine("Buy results:");
            Console.WriteLine();
            string buyResultStr = JsonConvert.SerializeObject(buyResult);
            Console.WriteLine(buyResultStr);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Sell results:");
            Console.WriteLine();
            string sellResultStr = JsonConvert.SerializeObject(sellResult);
            Console.WriteLine(sellResultStr);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        public void OutputString(string toOutput)
        {
            Console.WriteLine(toOutput);
        }
    }
}