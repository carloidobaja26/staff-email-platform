namespace EmailPlatform.Domain.Enums;

public enum EmailJobStatus
{
    Pending = 1,
    Processing = 2,
    Sent = 3,
    Failed = 4,
    Cancelled = 5
}