using TechnicalTest.Application.Common;

namespace TechnicalTest.Application.Reservations;

public interface IReservationService
{
    Task<Result<ReservationDto>> CreateAsync(CreateReservationCommand command, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<ReservationDto>>> GetAllAsync(CancellationToken cancellationToken);

    Task<Result<ReservationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<ReservationDto>> ConfirmAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<ReservationDto>> CancelAsync(Guid id, CancellationToken cancellationToken);
}