namespace Banks.Entities.Builders.Bank;

public interface IDepositExpirationDateBuilder
{
    IBankBuilder WithDepositExpirationDate(DateTime expirationDate);
}