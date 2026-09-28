namespace EmailPlatformApi.Smtp;

public sealed class SmtpOptions
{
    public bool Enabled { get; set; } = true;

    public int Port { get; set; } = 587;

    public string ServerName { get; set; } = "localhost";

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string CertificatePath { get; set; } = string.Empty;

    public string CertificatePassword { get; set; } = string.Empty;
}