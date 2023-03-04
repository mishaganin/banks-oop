using Banks.Models;
using Banks.Services;

namespace Banks.Console.Parsers;

public class BankCreator : AbsParser
{
    public override string Execute(string command)
    {
        if (command == "create bank")
        {
            System.Console.WriteLine("Enter a name:");
            string name = System.Console.ReadLine() ?? throw new Exception("No value((");

            System.Console.WriteLine("Enter a address:");
            string address = System.Console.ReadLine() ?? throw new Exception("No value((");

            System.Console.WriteLine("Enter a capital:");
            decimal capital = Convert.ToDecimal(System.Console.ReadLine() ?? throw new Exception("No value(("));

            System.Console.WriteLine("Enter a debit percent:");
            decimal debitPercent = Convert.ToDecimal(System.Console.ReadLine() ?? throw new Exception("No value(("));

            DateTime depositExpirationDate = DateTime.Now;

            System.Console.WriteLine("Enter deposit percents:");
            List<decimal> depositPercents = System.Console.ReadLine()?
                                                  .Split(' ')
                                                  .Select(percent => Convert.ToDecimal(percent)).ToList()
                                            ?? new List<decimal>();

            System.Console.WriteLine("Enter deposit limits:");
            List<decimal> depositLimits = System.Console.ReadLine()?
                                                  .Split(' ')
                                                  .Select(percent => Convert.ToDecimal(percent)).ToList()
                                            ?? new List<decimal>();

            System.Console.WriteLine("Enter a credit percent:");
            decimal creditPercent = Convert.ToDecimal(System.Console.ReadLine() ?? throw new Exception("No value(("));

            System.Console.WriteLine("Enter a withdraw limit:");
            decimal withdrawLimit = Convert.ToDecimal(System.Console.ReadLine() ?? throw new Exception("No value(("));

            System.Console.WriteLine("Enter a credit commission:");
            decimal creditCommission = Convert.ToDecimal(System.Console.ReadLine() ?? throw new Exception("No value(("));

            Guid bankId = CentralBank.GetConnection().CreateBank(
                name,
                address,
                capital,
                debitPercent,
                depositPercents,
                depositLimits,
                creditPercent,
                withdrawLimit,
                creditCommission,
                depositExpirationDate).Id;
            System.Console.WriteLine($"Bank's id: {bankId}");
            return "Bank successfully created!";
        }

        return base.Execute(command);
    }
}