using Banks.Entities.Builders;
using Banks.Entities.Builders.Client;
using Banks.Entities.Observers.Notify;
using Banks.Models;

namespace Banks.Entities;

public class Client : INotifyObserver
{
    private List<IAccount> _accounts = new List<IAccount>();
    private List<Notification> _notifications = new List<Notification>();
    private Client(
        string name,
        string surname,
        string? address,
        Passport? passport)
    {
        Name = name;
        Surname = surname;
        Address = address;
        Passport = passport;
        Id = Guid.NewGuid();
    }

    public static INameBuilder Builder => new ClientBuilder();
    public string Name { get; }
    public string Surname { get; }
    public string? Address { get; private set; }
    public Passport? Passport { get; private set; }
    public Guid Id { get; }

    public IReadOnlyList<IAccount> Accounts => _accounts.AsReadOnly();
    public void Update(Notification notification)
    {
        _notifications.Add(notification);
    }

    public void AddAccount(IAccount account)
    {
        _accounts.Add(account);
    }

    public Client AddAddress(string address)
    {
        Address = address;
        return this;
    }

    public Client AddPassport(Passport passport)
    {
        Passport = passport;
        return this;
    }

    private class ClientBuilder : INameBuilder, ISurnameBuilder, IClientBuilder
    {
        public ClientBuilder()
        {
            Name = string.Empty;
            Surname = string.Empty;
            Address = null;
            Passport = null;
        }

        private string Name { get; set; }
        private string Surname { get; set; }
        private string? Address { get; set; }
        private Passport? Passport { get; set; }

        public ISurnameBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        public IClientBuilder WithSurname(string surname)
        {
            Surname = surname;
            return this;
        }

        public IClientBuilder WithAddress(string? address)
        {
            Address = address;
            return this;
        }

        public IClientBuilder WithPassport(Passport? passport)
        {
            Passport = passport;
            return this;
        }

        public Client Build()
        {
            return new Client(
                Name,
                Surname,
                Address,
                Passport);
        }
    }
}