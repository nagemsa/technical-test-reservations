using TechnicalTest.Domain.Reservations;

namespace TechnicalTest.Application.Abstractions;

public interface IReservationRepository
{
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken);

    Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken cancellationToken);

    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsDuplicateAsync(
        string customerName,
        string origin,
        string destination,
        DateTime reservationDateUtc,
        ServiceType serviceType,
        CancellationToken cancellationToken);

    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken);
}