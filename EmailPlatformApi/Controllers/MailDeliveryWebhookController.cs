using EmailPlatform.Application.Common.Pagination;
using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using EmailPlatformApi.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EmailPlatformApi.Controllers;

[ApiController]
[Route("api/webhooks/maildelivery")]
public class MailDeliveryWebhookController : BaseController
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

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] PaginationRequest request,
        [FromQuery] Guid emailJobId)
    {
        return HandleResponse(
            await _webhookService.GetPagedAsync(
                emailJobId,
                request));
    }
}
