using Microsoft.Extensions.Options;
using SmtpServer;
using SmtpServer.ComponentModel;
using SmtpServer.Storage;

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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SMTP ExecuteAsync started.");

        if (!_options.Enabled)
        {
            _logger.LogWarning("SMTP server is disabled in appsettings.");
            return;
        }

        try
        {
            // Bind to internal port 2525 across all network interfaces inside the container
            const int internalPort = 2525;

            var serverOptions = new SmtpServerOptionsBuilder()
                .ServerName(_options.ServerName)
                .Endpoint(ep => ep.Port(internalPort, isSecure: false))
                .Build();

            // Adapt container for SmtpServer
            var container = new SmtpServer.ComponentModel.ServiceProvider();
            container.Add(_serviceProvider.GetRequiredService<IMessageStore>());

            _smtpServer = new SmtpServer.SmtpServer(serverOptions, container);

            _logger.LogInformation(
                "Starting SMTP server internally on port {InternalPort} (Railway external proxy port: {ExternalPort})...", 
                internalPort, 
                _options.Port);

            await _smtpServer.StartAsync(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("SMTP server stopped due to application shutdown.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "SMTP server failed to start.");
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping SMTP server.");
        try
        {
            _smtpServer?.Shutdown();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error shutting down SMTP server.");
        }

        return base.StopAsync(cancellationToken);
    }
}