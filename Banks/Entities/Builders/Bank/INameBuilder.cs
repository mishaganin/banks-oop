namespace Banks.Entities.Builders.Bank;

public interface INameBuilder
{
    IAddressBuilder WithName(string name);
}