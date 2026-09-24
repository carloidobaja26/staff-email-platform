using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;

namespace EmailPlatform.Infrastructure.EmailProviders;

public class EmailDeliveryProvider : IEmailProvider
{
    private readonly EmailDeliveryOptions _options;
    private readonly ILogger<EmailDeliveryProvider> _logger;

    public EmailDeliveryProvider(
        IOptions<EmailDeliveryOptions> options,
        ILogger<EmailDeliveryProvider> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public string Name => "emaildelivery";

public async Task<EmailSendResult> SendAsync(
    EmailMessage message,
    CancellationToken cancellationToken = default)
{
    _logger.LogInformation(
        "EmailDeliveryProvider START. To={To}, Subject={Subject}",
        message.To,
        message.Subject);

    try
    {
        var mimeMessage = new MimeMessage();

        mimeMessage.MessageId = MimeUtils.GenerateMessageId();

        var fromAddress =
            message.From ?? _options.FromAddress;

        mimeMessage.From.Add(
            new MailboxAddress(
                _options.FromName,
                fromAddress));

        mimeMessage.To.Add(
            MailboxAddress.Parse(message.To));

        mimeMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = message.HtmlBody,
            TextBody = message.TextBody
        };

        mimeMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        _logger.LogInformation(
            "Connecting to EmailDelivery SMTP {Host}:{Port}",
            _options.Host,
            _options.Port);

        await client.ConnectAsync(
            _options.Host,
            _options.Port,
            SecureSocketOptions.StartTls,
            cancellationToken);

        _logger.LogInformation(
            "Connected to EmailDelivery SMTP.");

        await client.AuthenticateAsync(
            _options.Username,
            _options.Password,
            cancellationToken);

        _logger.LogInformation(
            "Authenticated with EmailDelivery SMTP.");

        await client.SendAsync(
            mimeMessage,
            cancellationToken);

        _logger.LogInformation(
            "SMTP SEND completed. MessageId={MessageId}",
            mimeMessage.MessageId);

        await client.DisconnectAsync(
            true,
            cancellationToken);

        return new EmailSendResult
        {
            Success = true,
            IsAccepted = true,
            ProviderMessageId = mimeMessage.MessageId
        };
    }
    catch (SmtpCommandException ex)
    {
        _logger.LogError(
            ex,
            "EmailDelivery SMTP command failed.");

        return new EmailSendResult
        {
            Success = false,
            IsTransient = true,
            ErrorCode = ex.StatusCode.ToString(),
            ErrorMessage = ex.Message
        };
    }
    catch (SmtpProtocolException ex)
    {
        _logger.LogError(
            ex,
            "EmailDelivery SMTP protocol failed.");

        return new EmailSendResult
        {
            Success = false,
            IsTransient = true,
            ErrorCode = "SMTP_PROTOCOL_ERROR",
            ErrorMessage = ex.Message
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "EmailDelivery SMTP unexpected error.");

        return new EmailSendResult
        {
            Success = false,
            IsTransient = true,
            ErrorCode = "SMTP_ERROR",
            ErrorMessage = ex.Message
        };
    }
}
    private static bool IsTransient(
        SmtpStatusCode statusCode)
    {
        return statusCode switch
        {
            SmtpStatusCode.ServiceNotAvailable => true,
            SmtpStatusCode.MailboxBusy => true,
            SmtpStatusCode.MailboxUnavailable => true,
            SmtpStatusCode.TransactionFailed => true,
            _ => false
        };
    }
}