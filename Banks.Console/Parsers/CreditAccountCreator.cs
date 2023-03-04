using Banks.Models;
using Banks.Services;

namespace Banks.Console.Parsers;

public class CreditAccountCreator : AbsParser
{
    public override string Execute(string command)
    {
        if (command == "create credit account")
        {
            System.Console.WriteLine("Set data about credit account:");

            System.Console.WriteLine("Enter a balance:");
            decimal balance = Convert.ToDecimal(System.Console.ReadLine() ?? throw new Exception("No value(("));

            System.Console.WriteLine("Enter a bank id:");
            string bankId = System.Console.ReadLine() ?? throw new Exception("No value((");

            System.Console.WriteLine("Enter a client id:");
            string clientId = System.Console.ReadLine() ?? throw new Exception("No value((");

            Guid accountId = CentralBank.GetConnection().GetBankById(new Guid(bankId))
                                        .CreateCreditAccount(balance, new Guid(clientId)).Id;
            System.Console.WriteLine($"Credit account's id: {accountId}");
            return "Credit account successfully created!";
        }

        return base.Execute(command);
    }
}