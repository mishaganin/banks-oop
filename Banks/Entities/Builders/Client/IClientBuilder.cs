using Banks.Models;

namespace Banks.Entities.Builders.Client;

public interface IClientBuilder
{
    IClientBuilder WithAddress(string? address);
    IClientBuilder WithPassport(Passport? passport);

    Entities.Client Build();
}