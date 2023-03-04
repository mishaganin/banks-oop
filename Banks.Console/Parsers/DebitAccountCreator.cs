using Banks.Models;
using Banks.Services;

namespace Banks.Console.Parsers;

public class DebitAccountCreator : AbsParser
{
    public override string Execute(string command)
    {
        if (command == "create debit account")
        {
            System.Console.WriteLine("Set data about debit account:");

            System.Console.WriteLine("Enter a balance");
            decimal balance = Convert.ToDecimal(System.Console.ReadLine() ?? throw new Exception("No value(("));

            System.Console.WriteLine("Enter a bank id");
            string bankId = System.Console.ReadLine() ?? throw new Exception("No value((");

            System.Console.WriteLine("Enter a client id");
            string clientId = System.Console.ReadLine() ?? throw new Exception("No value((");

            Guid accountId = CentralBank.GetConnection().GetBankById(new Guid(bankId))
                                       .CreateDebitAccount(balance, new Guid(clientId)).Id;
            System.Console.WriteLine($"Debit account's id: {accountId}");
            return "Debit account successfully created!";
        }

        return base.Execute(command);
    }
}