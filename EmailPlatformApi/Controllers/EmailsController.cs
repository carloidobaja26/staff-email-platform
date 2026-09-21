using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmailPlatformApi.Controllers;

[ApiController]
[Route("api/emails")]
public class EmailsController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailsController(
        IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> Queue(
        [FromBody] QueueEmailRequest request,
        CancellationToken cancellationToken)
    {
        var emailJobId =
            await _emailService.QueueAsync(
                request,
                cancellationToken);

        return Ok(new
        {
            success = true,
            data = new
            {
                emailJobId
            }
        });
    }
}