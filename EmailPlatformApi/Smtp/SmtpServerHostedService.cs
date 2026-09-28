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
        _logger.LogInformation(
            "SMTP ExecuteAsync started.");

        _logger.LogInformation(
            "SMTP Enabled={Enabled}, Port={Port}, ServerName={ServerName}",
            _options.Enabled,
            _options.Port,
            _options.ServerName);

        if (!_options.Enabled)
        {
            _logger.LogWarning(
                "SMTP server is disabled.");

            return;
        }

        try
        {
            var serverOptions = new SmtpServerOptionsBuilder()
                .ServerName(_options.ServerName)
                .Port(_options.Port)
                .Build();

            _logger.LogInformation(
                "SMTP options built successfully.");

            _smtpServer = new SmtpServer.SmtpServer(
                serverOptions,
                _serviceProvider);

            _logger.LogInformation(
                "SMTP server object created.");

            _logger.LogInformation(
                "Calling SmtpServer.StartAsync on port {Port}.",
                _options.Port);

            await _smtpServer.StartAsync(
                stoppingToken);

            _logger.LogWarning(
                "SmtpServer.StartAsync returned normally.");
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "SMTP server stopped because the application is shutting down.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(
                ex,
                "SmtpServer.StartAsync failed.");
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
                "Error while shutting down SMTP server.");
        }

        return base.StopAsync(
            cancellationToken);
    }
}