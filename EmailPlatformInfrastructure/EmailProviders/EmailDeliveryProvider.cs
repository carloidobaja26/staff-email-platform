using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

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
            "EmailDeliveryProvider START. " +
            "EmailJobId={EmailJobId}, To={To}, Subject={Subject}",
            message.EmailJobId,
            message.To,
            message.Subject);

        try
        {
            var mimeMessage = new MimeMessage();

            var fromAddress =
                string.IsNullOrWhiteSpace(message.From)
                    ? _options.FromAddress
                    : message.From;

            mimeMessage.From.Add(
                new MailboxAddress(
                    _options.FromName,
                    fromAddress));

            mimeMessage.To.Add(
                MailboxAddress.Parse(message.To));

            mimeMessage.Subject = message.Subject;

            /*
             * Use our EmailJob ID as the MIME Message-ID.
             *
             * Example:
             * <emailjob-860202b495a5488da39154a389cb418c@mail.staffconnect.app>
             */
            mimeMessage.MessageId =
                $"<emailjob-{message.EmailJobId:N}@mail.staffconnect.app>";

            _logger.LogInformation(
                "Created MIME Message-ID. " +
                "EmailJobId={EmailJobId}, MessageId={MessageId}",
                message.EmailJobId,
                mimeMessage.MessageId);

            /*
             * Do NOT send X-Transactional-Tag yet.
             *
             * EmailDelivery previously returned:
             * 501 Invalid tag.
             *
             * We need to determine the provider's accepted tag format
             * before enabling it again.
             */

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message.HtmlBody,
                TextBody = message.TextBody
            };

            mimeMessage.Body =
                bodyBuilder.ToMessageBody();

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
                "SMTP SEND completed. " +
                "EmailJobId={EmailJobId}, MessageId={MessageId}",
                message.EmailJobId,
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
                "EmailDelivery SMTP command failed. " +
                "EmailJobId={EmailJobId}, StatusCode={StatusCode}",
                message.EmailJobId,
                ex.StatusCode);

            return new EmailSendResult
            {
                Success = false,
                IsTransient = IsTransient(ex.StatusCode),
                ErrorCode = ex.StatusCode.ToString(),
                ErrorMessage = ex.Message
            };
        }
        catch (SmtpProtocolException ex)
        {
            _logger.LogError(
                ex,
                "EmailDelivery SMTP protocol failed. " +
                "EmailJobId={EmailJobId}",
                message.EmailJobId);

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
                "EmailDelivery SMTP unexpected error. " +
                "EmailJobId={EmailJobId}",
                message.EmailJobId);

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