namespace Banks.Console.Interfaces;

public interface IParser
{
    IParser SetNextParser(IParser parser);
    string Execute(string command);
}