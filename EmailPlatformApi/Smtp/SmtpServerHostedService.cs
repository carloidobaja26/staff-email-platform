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
            // Explicitly define endpoint port binding
            var serverOptions = new SmtpServerOptionsBuilder()
                .ServerName(_options.ServerName)
                .Endpoint(ep => ep.Port(_options.Port, isSecure: false))
                .Build();

            // Adapt ASP.NET Core IServiceProvider to SmtpServer's ServiceProvider
            var container = new SmtpServer.ComponentModel.ServiceProvider();
            container.Add(_serviceProvider.GetRequiredService<SmtpServer.Storage.IMessageStore>());

            _smtpServer = new SmtpServer.SmtpServer(serverOptions, container);

            _logger.LogInformation("Starting SMTP server on port {Port}...", _options.Port);

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