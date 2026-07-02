using System.Globalization;
using TechnicalTest.Domain.Reservations;

namespace TechnicalTest.Tests;

public sealed class ReservationPricingCalculatorTests
{
    [Theory]
    [InlineData("2026-04-18T10:00:00Z", "2026-04-18T09:00:00Z", 2, ServiceType.Standard, 84_000)]
    [InlineData("2026-04-22T10:00:00Z", "2026-04-18T10:00:00Z", 5, ServiceType.Premium, 156_228)]
    public void Calculate_ReturnsExpectedAmounts(string reservationDate, string now, int passengers, ServiceType serviceType, decimal expected)
    {
        var price = ReservationPricingCalculator.Calculate(
            DateTime.Parse(reservationDate, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
            passengers,
            serviceType,
            DateTime.Parse(now, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal));

        Assert.Equal(expected, price);
    }
}