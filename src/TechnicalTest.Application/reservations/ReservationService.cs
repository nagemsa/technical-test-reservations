using TechnicalTest.Application.Abstractions;
using TechnicalTest.Application.Common;
using TechnicalTest.Domain.Reservations;

namespace TechnicalTest.Application.Reservations;

public sealed class ReservationService : IReservationService
{
    private const string InvalidStatusMessage = "Reservation cannot be modified in its current status.";

    private readonly IClock _clock;
    private readonly IReservationRepository _reservationRepository;

    public ReservationService(IClock clock, IReservationRepository reservationRepository)
    {
        _clock = clock;
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<ReservationDto>> CreateAsync(CreateReservationCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var utcNow = _clock.UtcNow;
        var reservationDateUtc = DateTime.SpecifyKind(command.Date, DateTimeKind.Utc);
        var validationErrors = Validate(command, reservationDateUtc, utcNow);

        if (validationErrors.Count > 0)
        {
            return Result<ReservationDto>.Failure(validationErrors.ToArray());
        }

        var customerName = Normalize(command.CustomerName!);
        var origin = Normalize(command.Origin!);
        var destination = Normalize(command.Destination!);

        var duplicateExists = await _reservationRepository.ExistsDuplicateAsync(
            customerName,
            origin,
            destination,
            reservationDateUtc,
            command.ServiceType,
            cancellationToken);

        if (duplicateExists)
        {
            return Result<ReservationDto>.Failure(Error.Conflict("An identical reservation already exists."));
        }

        var totalPrice = ReservationPricingCalculator.Calculate(reservationDateUtc, command.Passengers, command.ServiceType, utcNow);
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            customerName,
            origin,
            destination,
            reservationDateUtc,
            command.Passengers,
            command.ServiceType,
            totalPrice,
            utcNow);

        await _reservationRepository.AddAsync(reservation, cancellationToken);
        return Result<ReservationDto>.Success(Map(reservation));
    }

    public async Task<Result<IReadOnlyList<ReservationDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.GetAllAsync(cancellationToken);
        var response = reservations.Select(Map).ToList();
        return Result<IReadOnlyList<ReservationDto>>.Success(response);
    }

    public async Task<Result<ReservationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, cancellationToken);

        if (reservation is null)
        {
            return Result<ReservationDto>.Failure(Error.NotFound());
        }

        return Result<ReservationDto>.Success(Map(reservation));
    }

    public async Task<Result<ReservationDto>> ConfirmAsync(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, cancellationToken);

        if (reservation is null)
        {
            return Result<ReservationDto>.Failure(Error.NotFound());
        }

        if (reservation.Status != ReservationStatus.Created)
        {
            return Result<ReservationDto>.Failure(Error.InvalidState(InvalidStatusMessage));
        }

        reservation.Confirm(_clock.UtcNow);
        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        return Result<ReservationDto>.Success(Map(reservation));
    }

    public async Task<Result<ReservationDto>> CancelAsync(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, cancellationToken);

        if (reservation is null)
        {
            return Result<ReservationDto>.Failure(Error.NotFound());
        }

        if (reservation.Status != ReservationStatus.Created)
        {
            return Result<ReservationDto>.Failure(Error.InvalidState(InvalidStatusMessage));
        }

        reservation.Cancel(_clock.UtcNow);
        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        return Result<ReservationDto>.Success(Map(reservation));
    }

    private static List<Error> Validate(CreateReservationCommand command, DateTime reservationDateUtc, DateTime utcNow)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(command.CustomerName))
        {
            errors.Add(Error.Validation("CustomerName is required."));
        }

        if (string.IsNullOrWhiteSpace(command.Origin))
        {
            errors.Add(Error.Validation("Origin is required."));
        }

        if (string.IsNullOrWhiteSpace(command.Destination))
        {
            errors.Add(Error.Validation("Destination is required."));
        }

        if (!Enum.IsDefined(typeof(ServiceType), command.ServiceType))
        {
            errors.Add(Error.Validation("ServiceType must be Standard or Premium."));
        }

        if (command.Passengers is < 1 or > 6)
        {
            errors.Add(Error.Validation("Passengers must be between 1 and 6."));
        }

        if (reservationDateUtc <= utcNow)
        {
            errors.Add(Error.Validation("Date must be in the future."));
        }

        if (!string.IsNullOrWhiteSpace(command.Origin) && !string.IsNullOrWhiteSpace(command.Destination) &&
            string.Equals(command.Origin.Trim(), command.Destination.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(Error.Validation("Origin and destination must be different."));
        }

        return errors;
    }

    private static string Normalize(string value)
    {
        return value.Trim();
    }

    private static ReservationDto Map(Reservation reservation)
    {
        return new ReservationDto(
            reservation.Id,
            reservation.CustomerName,
            reservation.Origin,
            reservation.Destination,
            reservation.Date,
            reservation.Passengers,
            reservation.ServiceType,
            reservation.Status,
            reservation.TotalPrice);
    }
}