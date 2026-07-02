using TechnicalTest.Application.Reservations;
using TechnicalTest.Domain.Reservations;

namespace TechnicalTest.Api.Contracts;

public sealed record CreateReservationRequest(
    string? CustomerName,
    string? Origin,
    string? Destination,
    DateTime Date,
    int Passengers,
    ServiceType ServiceType)
{
    public CreateReservationCommand ToCommand()
    {
        return new CreateReservationCommand(CustomerName, Origin, Destination, Date, Passengers, ServiceType);
    }
}