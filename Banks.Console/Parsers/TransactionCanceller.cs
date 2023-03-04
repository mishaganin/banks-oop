using Banks.Models;
using Banks.Services;

namespace Banks.Console.Parsers;

public class TransactionCanceller : AbsParser
{
    public override string Execute(string command)
    {
        if (command == "undo transaction")
        {
            System.Console.WriteLine("Enter a bank id:");
            string bankId = System.Console.ReadLine() ?? throw new Exception("No value((");

            System.Console.WriteLine("Enter a transaction id:");
            string transactionId = System.Console.ReadLine() ?? throw new Exception("No value((");

            CentralBank.GetConnection().GetBankById(new Guid(bankId)).UndoLastTransaction(new Guid(transactionId));
            return "Transaction successfully cancelled!";
        }

        return base.Execute(command);
    }
}