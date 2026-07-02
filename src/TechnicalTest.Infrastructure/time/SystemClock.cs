using TechnicalTest.Application.Abstractions;

namespace TechnicalTest.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}