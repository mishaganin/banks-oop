using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Models;
using Banks.Services;
using Xunit;

namespace Banks.Test;

public class BanksTests
{
    [Fact]
    public void CreateAccounts()
    {
        CentralBank cb = CentralBank.GetConnection();
        Bank bank1 = cb.CreateBank(
            "Sber",
            "Kronverksky, 49",
            100000,
            2,
            new List<decimal>() { 2, 3.5m, 4.5m },
            new List<decimal>() { 10000, 25000 },
            7,
            15000,
            300,
            DateTime.Now);
        Client client1 = bank1.AddClient("Misha", "Spb");
        bank1.AddClientPassport(client1.Id, new Passport(1234, 567890));
        bank1.AddClientAddress(client1.Id, "sweet home");
        IAccount debitAccount = bank1.CreateDebitAccount(25000, client1.Id);
        cb.PassDays(5);
        cb.PassDays(25);
        debitAccount.AccrueAllDeposits();
        Guid id1 = bank1.MakeTransaction(0, debitAccount.Id, 12000, null);
        bank1.UndoLastTransaction(id1);
        bank1.MakeTransaction(0, debitAccount.Id, 25000, null);
        Guid id2 = bank1.MakeTransaction(1, debitAccount.Id, 123000, null);
        bank1.UndoLastTransaction(id2);
        CreditAccount creditAccount = bank1.CreateCreditAccount(500, client1.Id);
        Guid id3 = bank1.MakeTransaction(0, creditAccount.Id, 15400, null);
        cb.PassDays(32);
        bank1.UndoLastTransaction(id3);
        Assert.Equal(9600, creditAccount.Commission);
        Bank bank2 = cb.CreateBank(
            "Alfa",
            "Street, 239",
            50000,
            3.7m,
            new List<decimal>() { 2, 3.5m, 4.5m },
            new List<decimal>() { 10000, 25000 },
            7,
            15000,
            300,
            DateTime.Now);
        Client client2 = bank2.AddClient("Name1", "Surname1", "Address1");
    }
}