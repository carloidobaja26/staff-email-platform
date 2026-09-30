using EmailPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmailPlatformApi.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IEmailRetryWorker _retryWorker;

    public TestController(IEmailRetryWorker retryWorker)
    {
        _retryWorker = retryWorker;
    }

    [HttpGet("email-retry")]
    public async Task<IActionResult> TestRetry(
        CancellationToken cancellationToken)
    {
        await _retryWorker.ProcessAsync(cancellationToken);

        return Ok(new
        {
            success = true
        });
    }
}