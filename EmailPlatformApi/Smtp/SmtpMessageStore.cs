using System.Buffers;
using EmailPlatform.Application.Interfaces;
using EmailPlatform.Domain.Entities;
using EmailPlatform.Domain.Enums;
using Microsoft.Extensions.Options;
using MimeKit;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace EmailPlatformApi.Smtp;

public sealed class SmtpMessageStore : MessageStore
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<SmtpOptions> _options;
    private readonly ILogger<SmtpMessageStore> _logger;

    public SmtpMessageStore(
        IServiceScopeFactory scopeFactory,
        IOptions<SmtpOptions> options,
        ILogger<SmtpMessageStore> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    public override async Task<SmtpResponse> SaveAsync(
        ISessionContext context,
        IMessageTransaction transaction,
        ReadOnlySequence<byte> buffer,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var stream =
                new MemoryStream(buffer.ToArray());

            var message =
                await MimeMessage.LoadAsync(
                    stream,
                    cancellationToken);

            string fromAddresses =
                string.Join(", ", message.From);

            string toAddresses =
                string.Join(", ", message.To);

            string subject =
                message.Subject ?? "(No Subject)";

            string textBody =
                message.TextBody ?? string.Empty;

            string htmlBody =
                message.HtmlBody ?? string.Empty;

            _logger.LogInformation(
                """
                === SMTP MESSAGE RECEIVED ===
                From: {From}
                To: {To}
                Subject: {Subject}
                TextBody: {TextBody}
                HtmlBody: {HtmlBody}
                =============================
                """,
                fromAddresses,
                toAddresses,
                subject,
                textBody,
                htmlBody);

            var emailJob = new EmailJob
            {
                Id = Guid.NewGuid(),

                ApplicationId =
                    new Guid(),

                To = toAddresses,

                From = fromAddresses,

                Subject = subject,

                Template = "smtp",

                Payload = System.Text.Json.JsonSerializer.Serialize(
                    new
                    {
                        TextBody = textBody,
                        HtmlBody = htmlBody
                    }),

                Category = EmailCategory.Legacy,

                Priority = EmailPriority.Normal,

                Status = EmailJobStatus.Pending,

                Attempts = 0,

                MaxAttempts = 5,

                CreatedAt = DateTimeOffset.UtcNow
            };

            using var scope =
                _scopeFactory.CreateScope();

            var submissionService =
                scope.ServiceProvider
                    .GetRequiredService<IEmailSubmissionService>();

            await submissionService.SubmitAsync(
                emailJob,
                cancellationToken);

            _logger.LogInformation(
                "SMTP message converted to EmailJob {EmailJobId}.",
                emailJob.Id);

            return SmtpResponse.Ok;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                """
                SMTP → EmailJob FAILED.

                ExceptionType: {ExceptionType}
                Message: {ExceptionMessage}
                InnerException: {InnerException}
                """,
                ex.GetType().FullName,
                ex.Message,
                ex.InnerException?.Message);

            return SmtpResponse.TransactionFailed;
        }
    }
}