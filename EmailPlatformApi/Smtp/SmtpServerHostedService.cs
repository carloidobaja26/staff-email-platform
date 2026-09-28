using Microsoft.Extensions.Options;
using SmtpServer;
using SmtpServer.ComponentModel;

namespace EmailPlatformApi.Smtp;

public sealed class SmtpServerHostedService : BackgroundService
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

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "SMTP ExecuteAsync started.");

        if (!_options.Enabled)
        {
            _logger.LogWarning(
                "SMTP server is disabled.");

            return;
        }

        try
        {
            const int internalPort = 2525;

            var serverOptions = new SmtpServerOptionsBuilder()
                .ServerName(_options.ServerName)
                .Endpoint(endpoint =>
                    endpoint.Port(
                        internalPort,
                        isSecure: false))
                .Build();

            var messageStore =
                _serviceProvider.GetRequiredService<SmtpMessageStore>();

            var smtpServiceProvider =
                new SmtpServer.ComponentModel.ServiceProvider();

            smtpServiceProvider.Add(messageStore);

            _smtpServer = new SmtpServer.SmtpServer(
                serverOptions,
                smtpServiceProvider);

            _logger.LogInformation(
                "Starting SMTP server on port {Port}.",
                internalPort);

            await _smtpServer.StartAsync(
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "SMTP server stopped.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(
                ex,
                "SMTP server failed.");
        }
    }

    public override Task StopAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Stopping SMTP server.");

        try
        {
            _smtpServer?.Shutdown();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error shutting down SMTP server.");
        }

        return base.StopAsync(cancellationToken);
    }
}