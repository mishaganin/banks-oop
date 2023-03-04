namespace Banks.Entities.Builders.Bank;

public interface ICreditPercentBuilder
{
    IWithdrawLimitBuilder WithCreditPercent(decimal creditPercent);
}