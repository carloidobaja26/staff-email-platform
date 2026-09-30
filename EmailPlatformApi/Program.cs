using System.Net.Sockets;
using EmailPlatform.Infrastructure;
using EmailPlatform.Infrastructure.BackgroundJobs;
using EmailPlatformApi.Smtp;
using Hangfire;
using Microsoft.Extensions.Options;
using SmtpServer.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection("Smtp"));

builder.Services.AddSingleton<SmtpMessageStore>();
builder.Services.AddHostedService<SmtpServerHostedService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHangfireDashboard(
    "/hangfire",
    new DashboardOptions
    {
        Authorization = new[] { new AllowAllHangfireAuthorizationFilter() }
    });
HangfireJobScheduler.RegisterRecurringJobs();

// Dynamic SMTP test endpoint
app.MapGet("/smtp-test", async (IOptions<SmtpOptions> smtpOptions) =>
{
    var options = smtpOptions.Value;

    if (!options.Enabled)
    {
        return Results.BadRequest(new { success = false, message = "SMTP is disabled in configuration." });
    }

    try
    {
        using var client = new TcpClient();

        // Connect dynamically to local container on configured port
        await client.ConnectAsync("127.0.0.1", options.Port);

        return Results.Ok(new
        {
            success = true,
            port = options.Port,
            message = $"Successfully connected to internal SMTP listener on port {options.Port}."
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: $"Failed to connect to 127.0.0.1:{options.Port}. Error: {ex.Message}");
    }
});

// Remove or comment out HTTPS redirection on Railway unless custom SSL certificates are configured
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();