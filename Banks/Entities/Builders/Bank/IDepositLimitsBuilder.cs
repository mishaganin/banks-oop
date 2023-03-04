namespace Banks.Entities.Builders.Bank;

public interface IDepositLimitsBuilder
{
    ICreditPercentBuilder WithDepositLimits(List<decimal> depositLimits);
}