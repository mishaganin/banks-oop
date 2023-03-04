namespace Banks.Entities.Builders.Bank;

public interface ICreditCommissionBuilder
{
    IDepositExpirationDateBuilder WithCreditCommission(decimal creditCommission);
}