namespace TechnicalTest.Domain.Reservations;

public static class ReservationPricingCalculator
{
    private const decimal StandardBasePrice = 50_000m;
    private const decimal PremiumBasePrice = 80_000m;
    private const decimal PassengerPrice = 10_000m;
    private const decimal SameDayMultiplier = 1.20m;
    private const decimal LargeGroupMultiplier = 1.15m;
    private const decimal PremiumLargeGroupMultiplier = 1.10m;
    private const decimal EarlyBookingDiscountMultiplier = 0.95m;

    public static decimal Calculate(DateTime reservationDateUtc, int passengers, ServiceType serviceType, DateTime utcNow)
    {
        decimal total = serviceType == ServiceType.Standard ? StandardBasePrice : PremiumBasePrice;
        total += passengers * PassengerPrice;

        if (reservationDateUtc.Date == utcNow.Date)
        {
            total *= SameDayMultiplier;
        }

        if (passengers > 4)
        {
            total *= LargeGroupMultiplier;
        }

        if (serviceType == ServiceType.Premium && passengers > 3)
        {
            total *= PremiumLargeGroupMultiplier;
        }

        if (reservationDateUtc.Date >= utcNow.Date.AddDays(2))
        {
            total *= EarlyBookingDiscountMultiplier;
        }

        return decimal.Round(total, 0, MidpointRounding.AwayFromZero);
    }
}