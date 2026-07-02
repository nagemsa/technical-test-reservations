using TechnicalTest.Domain.Reservations;

namespace TechnicalTest.Application.Reservations;

public sealed record ReservationDto(
    Guid Id,
    string CustomerName,
    string Origin,
    string Destination,
    DateTime Date,
    int Passengers,
    ServiceType ServiceType,
    ReservationStatus Status,
    decimal TotalPrice);