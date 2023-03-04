namespace Banks.Entities.Builders.Bank;

public interface IDebitPercentBuilder
{
    IDepositPercentsBuilder WithDebitPercent(decimal debitPercent);
}