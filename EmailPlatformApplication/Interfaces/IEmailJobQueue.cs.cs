namespace EmailPlatform.Application.Interfaces;

public interface IEmailJobQueue
{
    string Enqueue(
        Guid emailJobId,
        string queue);
}