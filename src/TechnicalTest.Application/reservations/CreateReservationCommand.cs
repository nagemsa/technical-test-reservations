using TechnicalTest.Domain.Reservations;

namespace TechnicalTest.Application.Reservations;

public sealed record CreateReservationCommand(
    string? CustomerName,
    string? Origin,
    string? Destination,
    DateTime Date,
    int Passengers,
    ServiceType ServiceType);