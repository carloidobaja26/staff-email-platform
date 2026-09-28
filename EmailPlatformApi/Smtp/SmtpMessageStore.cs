using System.Buffers;
using MimeKit;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace EmailPlatformApi.Smtp;

public sealed class SmtpMessageStore : MessageStore
{
    private readonly ILogger<SmtpMessageStore> _logger;

    public SmtpMessageStore(ILogger<SmtpMessageStore> logger)
    {
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
            await using var stream = new MemoryStream(buffer.ToArray());

            var message = await MimeMessage.LoadAsync(stream, cancellationToken);

            string fromAddresses = string.Join(", ", message.From);
            string toAddresses = string.Join(", ", message.To);
            string subject = message.Subject ?? "(No Subject)";
            string textBody = message.TextBody ?? "(Empty)";
            string htmlBody = message.HtmlBody ?? "(Empty)";

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

            return SmtpResponse.Ok;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process incoming SMTP message.");
            return SmtpResponse.TransactionFailed;
        }
    }
}