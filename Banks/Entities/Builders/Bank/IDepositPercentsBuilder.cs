namespace Banks.Entities.Builders.Bank;

public interface IDepositPercentsBuilder
{
    IDepositLimitsBuilder WithDepositPercents(List<decimal> depositPercents);
}