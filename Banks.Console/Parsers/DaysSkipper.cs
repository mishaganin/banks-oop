using Banks.Console.Interfaces;
using Banks.Models;
using Banks.Services;

namespace Banks.Console.Parsers;

public class DaysSkipper : AbsParser
{
    public override string Execute(string command)
    {
        if (command == "pass days")
        {
            System.Console.WriteLine("Enter amount of days to pass:");
            int days = Convert.ToInt32(System.Console.ReadLine());
            CentralBank.GetConnection().PassDays(days);
            return $"{days} days successfully passed!";
        }

        return base.Execute(command);
    }
}