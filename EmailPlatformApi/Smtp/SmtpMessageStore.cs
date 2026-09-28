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
            "SMTP message received. From={From}, To={To}, Subject={Subject}",
            message.From,
            message.To,
            message.Subject);

        // We will create EmailJob here next.

        return SmtpResponse.Ok;
    }
}