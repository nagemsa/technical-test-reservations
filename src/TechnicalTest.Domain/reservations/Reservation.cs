namespace TechnicalTest.Domain.Reservations;

public sealed class Reservation
{
    private Reservation(
        Guid id,
        string customerName,
        string origin,
        string destination,
        DateTime dateUtc,
        int passengers,
        ServiceType serviceType,
        decimal totalPrice,
        DateTime createdAtUtc)
    {
        Id = id;
        CustomerName = customerName;
        Origin = origin;
        Destination = destination;
        Date = dateUtc;
        Passengers = passengers;
        ServiceType = serviceType;
        TotalPrice = totalPrice;
        Status = ReservationStatus.Created;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }

    public string CustomerName { get; private set; }

    public string Origin { get; private set; }

    public string Destination { get; private set; }

    public DateTime Date { get; private set; }

    public int Passengers { get; private set; }

    public ServiceType ServiceType { get; private set; }

    public ReservationStatus Status { get; private set; }

    public decimal TotalPrice { get; private set; }

    public DateTime CreatedAtUtc { get; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public static Reservation Create(
        Guid id,
        string customerName,
        string origin,
        string destination,
        DateTime dateUtc,
        int passengers,
        ServiceType serviceType,
        decimal totalPrice,
        DateTime createdAtUtc)
    {
        return new Reservation(id, customerName, origin, destination, dateUtc, passengers, serviceType, totalPrice, createdAtUtc);
    }

    public void Confirm(DateTime utcNow)
    {
        Status = ReservationStatus.Confirmed;
        UpdatedAtUtc = utcNow;
    }

    public void Cancel(DateTime utcNow)
    {
        Status = ReservationStatus.Cancelled;
        UpdatedAtUtc = utcNow;
    }
}