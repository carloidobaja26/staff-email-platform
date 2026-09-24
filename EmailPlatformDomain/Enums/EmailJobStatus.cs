namespace EmailPlatform.Domain.Enums;

public enum EmailJobStatus
{
    Pending = 1,
    Processing = 2,
    Accepted = 3,
    Delivered = 4,
    Failed = 5,
    Cancelled = 6
}