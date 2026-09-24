namespace EmailPlatform.Infrastructure.EmailProviders;

public class EmailDeliveryOptions
{
    public string Host { get; set; } = null!;

    public int Port { get; set; } = 587;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FromAddress { get; set; } = null!;

    public string? FromName { get; set; }

    public bool EnableSsl { get; set; } = true;
}