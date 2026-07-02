namespace TechnicalTest.Application.Common;

public sealed record Error(string Code, string Message)
{
    public static Error Validation(string message) => new("validation", message);

    public static Error NotFound(string message = "Reservation not found.") => new("not_found", message);

    public static Error Conflict(string message) => new("conflict", message);

    public static Error InvalidState(string message) => new("invalid_state", message);
}