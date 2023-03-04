using Banks.Console.Interfaces;
using Banks.Models;
using Banks.Services;

namespace Banks.Console.Parsers;

public class TransactionMaker : AbsParser
{
    public override string Execute(string command)
    {
        if (command == "make transaction")
        {
            System.Console.WriteLine("Enter a bank id");
            string bankId = System.Console.ReadLine() ?? throw new Exception("No value((");

            System.Console.WriteLine("Choose type of transaction:");
            System.Console.WriteLine("0 - Withdraw");
            System.Console.WriteLine("1 - Replenish");
            System.Console.WriteLine("2 - Transfer");
            int type = Convert.ToInt32(System.Console.ReadLine());

            System.Console.WriteLine("Enter deposit account id:");
            string accountId = System.Console.ReadLine() ?? throw new Exception("No value((");

            System.Console.WriteLine("Enter amount of money:");
            decimal amountOfMoney = Convert.ToDecimal(System.Console.ReadLine());

            string recipientId = string.Empty;

            if (type == 2)
            {
                System.Console.WriteLine("Enter a recipient account id");
                recipientId = System.Console.ReadLine() ?? throw new Exception("No value((");
            }

            Guid transactionId = CentralBank.GetConnection().GetBankById(new Guid(bankId)).MakeTransaction(
                type,
                new Guid(accountId),
                amountOfMoney,
                new Guid(recipientId));
            return $"Transaction id: {transactionId}";
        }

        return base.Execute(command);
    }
}