namespace Banks.Entities.Builders.Bank;

public interface IWithdrawLimitBuilder
{
    ICreditCommissionBuilder WithWithdrawLimit(decimal withdrawLimit);
}