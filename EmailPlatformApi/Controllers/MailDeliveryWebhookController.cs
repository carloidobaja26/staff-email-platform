using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmailPlatformApi.Controllers;

[ApiController]
[Route("api/webhooks/maildelivery")]
public class MailDeliveryWebhookController : ControllerBase
{
    private readonly IMailDeliveryWebhookService _webhookService;

    public MailDeliveryWebhookController(
        IMailDeliveryWebhookService webhookService)
    {
        _webhookService = webhookService;
    }

    [HttpPost]
    public async Task<IActionResult> Receive(
        [FromBody] MailDeliveryWebhookRequest request,
        CancellationToken cancellationToken)
    {
        await _webhookService.ProcessAsync(
            request,
            cancellationToken);

        return Ok(new
        {
            success = true
        });
    }
}