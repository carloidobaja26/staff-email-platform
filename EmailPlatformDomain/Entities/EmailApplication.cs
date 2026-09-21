namespace EmailPlatform.Domain.Entities;

public class EmailApplication
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<EmailJob> EmailJobs { get; set; } = new List<EmailJob>();
}