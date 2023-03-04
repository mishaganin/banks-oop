namespace Banks.Entities.Builders.Bank;

public interface IAddressBuilder
{
    ICapitalBuilder WithAddress(string address);
}