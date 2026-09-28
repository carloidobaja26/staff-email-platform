using Microsoft.Extensions.Options;
using SmtpServer;
using SmtpServer.Authentication;

namespace EmailPlatformApi.Smtp;

public sealed class SmtpUserAuthenticator : IUserAuthenticator
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpUserAuthenticator> _logger;

    public SmtpUserAuthenticator(
        IOptions<SmtpOptions> options,
        ILogger<SmtpUserAuthenticator> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<bool> AuthenticateAsync(
        ISessionContext context,
        string user,
        string password,
        CancellationToken cancellationToken)
    {
        var valid =
            string.Equals(
                user,
                _options.Username,
                StringComparison.Ordinal)
            &&
            string.Equals(
                password,
                _options.Password,
                StringComparison.Ordinal);

        if (!valid)
        {
            _logger.LogWarning(
                "SMTP authentication failed for user {Username}.",
                user);
        }
        else
        {
            _logger.LogInformation(
                "SMTP authentication succeeded for user {Username}.",
                user);
        }

        return Task.FromResult(valid);
    }
}