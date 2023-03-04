using Banks.Entities;

namespace Banks.Services;

public interface ICentralBank
{
    public Bank CreateBank(
        string name,
        string address,
        decimal capital,
        decimal debitPercent,
        List<decimal> depositPercents,
        List<decimal> depositLimits,
        decimal creditPercent,
        decimal withdrawLimit,
        decimal creditCommission,
        DateTime depositExpirationDate);
}