namespace Banks.Entities.Builders.Bank;

public interface ICapitalBuilder
{
    IDebitPercentBuilder WithCapital(decimal capital);
}