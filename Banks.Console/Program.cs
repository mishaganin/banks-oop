using Banks.Console.Interfaces;
using Banks.Console.Parsers;

void GiveCommand(IParser parser, string command)
{
    string str = parser.Execute(command);
    Console.WriteLine(str == string.Empty ? "Unknown command!" : str);
}

var bankCreator = new BankCreator();
var clientCreator = new ClientCreator();
var debitAccountCreator = new DebitAccountCreator();
var depositAccountCreator = new DepositAccountCreator();
var creditAccountCreator = new CreditAccountCreator();
var transactionMaker = new TransactionMaker();
var transactionCanceller = new TransactionCanceller();
var daysSkipper = new DaysSkipper();

bankCreator
    .SetNextParser(clientCreator)
    .SetNextParser(debitAccountCreator)
    .SetNextParser(depositAccountCreator)
    .SetNextParser(creditAccountCreator)
    .SetNextParser(transactionMaker)
    .SetNextParser(transactionCanceller)
    .SetNextParser(daysSkipper);

string? command = Console.ReadLine();
while (command != "quit")
{
    if (command != null)
    {
        GiveCommand(bankCreator, command);
    }

    command = Console.ReadLine();
}

Console.WriteLine("Program has finished!");