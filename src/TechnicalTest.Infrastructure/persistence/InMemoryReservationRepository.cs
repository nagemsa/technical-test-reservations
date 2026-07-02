using System.Collections.Concurrent;
using TechnicalTest.Application.Abstractions;
using TechnicalTest.Domain.Reservations;

namespace TechnicalTest.Infrastructure.Persistence;

public sealed class InMemoryReservationRepository : IReservationRepository
{
    private readonly ConcurrentDictionary<Guid, Reservation> _reservations = new();

    public Task AddAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        _reservations[reservation.Id] = reservation;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Reservation> reservations = _reservations.Values
            .OrderBy(reservation => reservation.Date)
            .ThenBy(reservation => reservation.CustomerName)
            .ToList();

        return Task.FromResult(reservations);
    }

    public Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_reservations.TryGetValue(id, out var reservation) ? reservation : null);
    }

    public Task<bool> ExistsDuplicateAsync(
        string customerName,
        string origin,
        string destination,
        DateTime reservationDateUtc,
        ServiceType serviceType,
        CancellationToken cancellationToken)
    {
        var duplicateExists = _reservations.Values.Any(reservation =>
            string.Equals(reservation.CustomerName, customerName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(reservation.Origin, origin, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(reservation.Destination, destination, StringComparison.OrdinalIgnoreCase) &&
            reservation.Date == reservationDateUtc &&
            reservation.ServiceType == serviceType);

        return Task.FromResult(duplicateExists);
    }

    public Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        _reservations[reservation.Id] = reservation;
        return Task.CompletedTask;
    }
}