using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmailPlatform.Api.Controllers;

[ApiController]
[Route("api/webhooks/maildelivery")]
public class MailDeliveryWebhookController : ControllerBase
{
    private readonly IMailDeliveryWebhookService _service;

    public MailDeliveryWebhookController(
        IMailDeliveryWebhookService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Handle(
        [FromBody] MailDeliveryWebhookRequest request,
        CancellationToken cancellationToken)
    {
        await _service.ProcessAsync(
            request,
            cancellationToken);

        return Ok(new
        {
            success = true
        });
    }
}