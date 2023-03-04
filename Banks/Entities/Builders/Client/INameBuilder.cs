namespace Banks.Entities.Builders.Client;

public interface INameBuilder
{
    ISurnameBuilder WithName(string name);
}