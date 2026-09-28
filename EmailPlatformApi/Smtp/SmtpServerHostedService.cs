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
            const int internalPort = 2525;

            var serverOptions = new SmtpServerOptionsBuilder()
                .ServerName(_options.ServerName)
                .Endpoint(ep => ep.Port(internalPort, isSecure: false))
                .Build();

            var container = new SmtpServer.ComponentModel.ServiceProvider();
            var messageStore = _serviceProvider.GetRequiredService<IMessageStore>();

            container.Add(messageStore);
            container.Add(new DelegatingMessageStoreFactory(context => messageStore));

            _smtpServer = new SmtpServer.SmtpServer(serverOptions, container);

            // Add lifecycle logging to capture connected sessions in Railway logs
            _smtpServer.SessionCreated += (s, e) =>
            {
                _logger.LogInformation("SMTP Client Connected: {RemoteEndPoint}", e.Context.EndpointDefinition.Endpoint);
            };

            _smtpServer.SessionFaulted += (s, e) =>
            {
                _logger.LogError(e.Exception, "SMTP Session Faulted.");
            };

            _smtpServer.SessionCompleted += (s, e) =>
            {
                _logger.LogInformation("SMTP Session Completed.");
            };

            _logger.LogInformation("Starting SMTP server internally on port {InternalPort}...", internalPort);

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