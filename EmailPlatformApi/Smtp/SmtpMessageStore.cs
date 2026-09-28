using System.Buffers;
using MimeKit;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace EmailPlatformApi.Smtp;

public sealed class SmtpMessageStore : MessageStore
{
    private readonly ILogger<SmtpMessageStore> _logger;

    public SmtpMessageStore(
        ILogger<SmtpMessageStore> logger)
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
            await using var stream = new MemoryStream();

            var position = buffer.GetPosition(0);

            while (buffer.TryGet(
                ref position,
                out var memory))
            {
                await stream.WriteAsync(
                    memory,
                    cancellationToken);
            }

            stream.Position = 0;

            var message = await MimeMessage.LoadAsync(
                stream,
                cancellationToken);

            _logger.LogInformation(
                """
                SMTP MESSAGE RECEIVED
                From: {From}
                To: {To}
                Subject: {Subject}
                TextBody: {TextBody}
                HtmlBody: {HtmlBody}
                """,
                string.Join(", ", message.From),
                string.Join(", ", message.To),
                message.Subject,
                message.TextBody,
                message.HtmlBody);

            return SmtpResponse.Ok;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process incoming SMTP message.");

            return SmtpResponse.TransactionFailed;
        }
    }
}