using System.Buffers;
using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
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

            var fromAddress = message.From
                .OfType<MailboxAddress>()
                .Select(x => x.Address)
                .FirstOrDefault();

            var toAddresses = message.To
                .OfType<MailboxAddress>()
                .Select(x => x.Address)
                .ToList();

            var subject =
                message.Subject ?? "(No Subject)";

            var textBody =
                message.TextBody ?? string.Empty;

            var htmlBody =
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
                fromAddress,
                string.Join(", ", toAddresses),
                subject,
                textBody,
                htmlBody);

            using var scope =
                _scopeFactory.CreateScope();

            var emailService =
                scope.ServiceProvider
                    .GetRequiredService<IEmailService>();

            var request = new QueueEmailRequest
            {
                ApplicationId =
                    Guid.Parse(
                        "11111111-1111-1111-1111-111111111112"),

                To = string.Join(",", toAddresses),

                From = fromAddress,

                Subject = subject,

                Template = "smtp",

                Payload =
                    System.Text.Json.JsonSerializer.Serialize(
                        new
                        {
                            TextBody = textBody,
                            HtmlBody = htmlBody
                        }),

                Category = (int)EmailCategory.Legacy,

                Priority = (int)EmailPriority.Normal,

                IdempotencyKey = null
            };

            var emailJobId =
                await emailService.QueueAsync(
                    request,
                    cancellationToken);

            _logger.LogInformation(
                "SMTP message queued as EmailJob {EmailJobId}.",
                emailJobId);

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
