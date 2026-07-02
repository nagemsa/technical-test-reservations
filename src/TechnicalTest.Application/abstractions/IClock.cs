namespace TechnicalTest.Application.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
}