using EmailPlatform.Infrastructure;
using EmailPlatform.Infrastructure.BackgroundJobs;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(
    builder.Configuration);

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