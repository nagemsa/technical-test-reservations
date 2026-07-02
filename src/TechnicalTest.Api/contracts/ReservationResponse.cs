using TechnicalTest.Application.Reservations;
using TechnicalTest.Domain.Reservations;

namespace TechnicalTest.Api.Contracts;

public sealed record ReservationResponse(
    Guid Id,
    string CustomerName,
    string Origin,
    string Destination,
    DateTime Date,
    int Passengers,
    ServiceType ServiceType,
    ReservationStatus Status,
    decimal TotalPrice)
{
    public static ReservationResponse From(ReservationDto dto)
    {
        return new ReservationResponse(
            dto.Id,
            dto.CustomerName,
            dto.Origin,
            dto.Destination,
            dto.Date,
            dto.Passengers,
            dto.ServiceType,
            dto.Status,
            dto.TotalPrice);
    }
}