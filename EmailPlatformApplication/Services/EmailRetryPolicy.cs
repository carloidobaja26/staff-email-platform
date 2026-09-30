using EmailPlatform.Application.Interfaces;

namespace EmailPlatform.Application.Services;

public sealed class EmailRetryPolicy
    : IEmailRetryPolicy
{
    private static readonly TimeSpan[] RetryDelays =
    {
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(15),
        TimeSpan.FromHours(1),
        TimeSpan.FromHours(6)
    };

    public bool CanRetry(
        int attempts,
        int maxAttempts)
    {
        return attempts < maxAttempts;
    }

    public DateTimeOffset CalculateNextAttempt(
        DateTimeOffset now,
        int attempts)
    {
        var index = Math.Clamp(
            attempts - 1,
            0,
            RetryDelays.Length - 1);

        return now.Add(RetryDelays[index]);
    }
}