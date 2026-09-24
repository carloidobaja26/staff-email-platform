namespace EmailPlatform.Domain.Enums;

public enum EmailDeliveryStatus
{
    Accepted = 1,
    Delivered = 2,
    Deferred = 3,
    Bounced = 4,
    Failed = 5
}