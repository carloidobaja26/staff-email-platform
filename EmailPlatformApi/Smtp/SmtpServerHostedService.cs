using Microsoft.Extensions.Options;
using SmtpServer;

namespace EmailPlatformApi.Smtp;

public sealed class SmtpServerHostedService : IHostedService
{
    private readonly SmtpOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SmtpServerHostedService> _logger;

    private SmtpServer.SmtpServer? _smtpServer;

    public SmtpServerHostedService(
        IOptions<SmtpOptions> options,
        IServiceProvider serviceProvider,
        ILogger<SmtpServerHostedService> logger)
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation(
                "SMTP server is disabled.");

            return;
        }

        var options = new SmtpServerOptionsBuilder()
            .ServerName(_options.ServerName)
            .Port(_options.Port)
            .Build();

        _smtpServer = new SmtpServer.SmtpServer(
            options,
            _serviceProvider);

        _logger.LogInformation(
            "Starting SMTP server on port {Port}.",
            _options.Port);

        await _smtpServer.StartAsync(
            cancellationToken);

        _logger.LogInformation(
            "SMTP server started on port {Port}.",
            _options.Port);
    }

    public Task StopAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Stopping SMTP server.");

        return Task.CompletedTask;
    }
}