using EmailPlatform.Domain.Entities;

namespace EmailPlatform.Application.Interfaces;

public interface IEmailSubmissionService
{
    Task<EmailJob> SubmitAsync(
        EmailJob emailJob,
        CancellationToken cancellationToken = default);
}