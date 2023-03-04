namespace Banks.Models;

public class Passport
{
    public Passport(int series, int number)
    {
        Series = series;
        Number = number;
    }

    public int Series { get; }
    public int Number { get; }
}