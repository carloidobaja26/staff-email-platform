using EmailPlatform.Infrastructure;
using EmailPlatform.Infrastructure.BackgroundJobs;
using EmailPlatformApi.Smtp;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection("Smtp"));

builder.Services.AddSingleton<SmtpMessageStore>();

builder.Services.AddHostedService<SmtpServerHostedService>();

var app = builder.Build();

// partial turn on for testing in railway
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHangfireDashboard(
    "/hangfire",
    new DashboardOptions
    {
        Authorization = new[]
        {
            new AllowAllHangfireAuthorizationFilter()
        }
    });

// }

app.UseHttpsRedirection();

app.MapControllers();

app.Run();