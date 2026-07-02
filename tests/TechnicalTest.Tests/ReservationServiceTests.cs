using TechnicalTest.Application.Abstractions;
using TechnicalTest.Application.Reservations;
using TechnicalTest.Domain.Reservations;
using TechnicalTest.Infrastructure.Persistence;

namespace TechnicalTest.Tests;

public sealed class ReservationServiceTests
{
    private static readonly DateTime FixedNow = new(2026, 04, 18, 10, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task CreateAsync_CalculatesExpectedPrice()
    {
        var service = BuildService();

        var result = await service.CreateAsync(
            new CreateReservationCommand("Juan Pérez", "Bogotá", "Aeropuerto El Dorado", new DateTime(2026, 04, 20, 10, 0, 0, DateTimeKind.Utc), 3, ServiceType.Standard),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(76_000m, result.Value!.TotalPrice);
    }

    [Fact]
    public async Task CreateAsync_RejectsDuplicateReservation()
    {
        var service = BuildService();

        var command = new CreateReservationCommand("Juan Pérez", "Bogotá", "Aeropuerto El Dorado", new DateTime(2026, 04, 20, 10, 0, 0, DateTimeKind.Utc), 3, ServiceType.Standard);

        var first = await service.CreateAsync(command, CancellationToken.None);
        var second = await service.CreateAsync(command, CancellationToken.None);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsFailure);
        Assert.Equal("conflict", second.Errors.First().Code);
        Assert.Contains("identical reservation", second.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ConfirmAsync_AllowsConfirmationOnlyOnce()
    {
        var service = BuildService();
        var createResult = await service.CreateAsync(
            new CreateReservationCommand("Juan Pérez", "Bogotá", "Aeropuerto El Dorado", new DateTime(2026, 04, 20, 10, 0, 0, DateTimeKind.Utc), 2, ServiceType.Premium),
            CancellationToken.None);

        var confirmation = await service.ConfirmAsync(createResult.Value!.Id, CancellationToken.None);
        var secondConfirmation = await service.ConfirmAsync(createResult.Value.Id, CancellationToken.None);

        Assert.True(confirmation.IsSuccess);
        Assert.Equal(ReservationStatus.Confirmed, confirmation.Value!.Status);
        Assert.True(secondConfirmation.IsFailure);
        Assert.Equal("invalid_state", secondConfirmation.Errors.First().Code);
        Assert.Contains("current status", secondConfirmation.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    private static IReservationService BuildService()
    {
        return new ReservationService(new FixedClock(FixedNow), new InMemoryReservationRepository());
    }

    private sealed class FixedClock : IClock
    {
        public FixedClock(DateTime utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTime UtcNow { get; }
    }
}