using Microsoft.Extensions.Options;
using SmtpServer;

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
        if (!_options.Enabled)
        {
            _logger.LogInformation(
                "SMTP server is disabled.");

            return;
        }

        _logger.LogInformation(
            "SMTP configuration: Port={Port}, ServerName={ServerName}",
            _options.Port,
            _options.ServerName);

        var options = new SmtpServerOptionsBuilder()
            .ServerName(_options.ServerName)
            .Port(_options.Port)
            .Build();

        _smtpServer = new SmtpServer.SmtpServer(
            options,
            _serviceProvider);

        _logger.LogInformation(
            "SMTP server instance created.");

        _logger.LogInformation(
            "Starting SMTP server on port {Port}.",
            _options.Port);

        try
        {
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

        _smtpServer?.Shutdown();

        return base.StopAsync(
            cancellationToken);
    }
}
