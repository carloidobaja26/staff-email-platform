namespace EmailPlatform.Application.Interfaces;

public interface IEmailRetryPolicy
{
    bool CanRetry(
        int attempts,
        int maxAttempts);

    DateTimeOffset CalculateNextAttempt(
        DateTimeOffset now,
        int attempts);
}
