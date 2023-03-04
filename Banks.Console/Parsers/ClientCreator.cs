using Banks.Entities;
using Banks.Models;
using Banks.Services;

namespace Banks.Console.Parsers;

public class ClientCreator : AbsParser
{
    public override string Execute(string command)
    {
        if (command == "add client")
        {
            System.Console.WriteLine("Enter a bank id");
            string bankId = System.Console.ReadLine() ?? throw new Exception("No value((");

            System.Console.WriteLine("Set data about client:");

            System.Console.WriteLine("Enter a name:");
            string name = System.Console.ReadLine() ?? throw new Exception("No value((");

            System.Console.WriteLine("Enter a surname:");
            string surname = System.Console.ReadLine() ?? throw new Exception("No value((");

            System.Console.WriteLine("Enter an address:");
            string? address = System.Console.ReadLine();

            System.Console.WriteLine("Enter a series:");
            int series = Convert.ToInt32(System.Console.ReadLine());

            System.Console.WriteLine("Enter a number:");
            int number = Convert.ToInt32(System.Console.ReadLine());

            Passport passport = new Passport(series, number);
            Guid clientId = CentralBank.GetConnection().GetBankById(new Guid(bankId))
                                       .AddClient(name, surname, address, passport).Id;
            System.Console.WriteLine($"Client's id: {clientId}");
            return "Client successfully added!";
        }

        return base.Execute(command);
    }
}