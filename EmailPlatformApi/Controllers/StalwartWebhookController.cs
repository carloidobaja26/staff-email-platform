
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace EmailPlatformApi.Controllers;

[ApiController]
[Route("api/webhooks/stalwart")]
public class StalwartWebhookController : ControllerBase
{
    private readonly ILogger<StalwartWebhookController> _logger;

    public StalwartWebhookController(
        ILogger<StalwartWebhookController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Receive([FromBody] JsonElement payload)
    {
        var json = payload.GetRawText();
        _logger.LogInformation(
            "Stalwart webhook trigger");
        _logger.LogInformation(
            "Stalwart webhook received Payload {Payload}",
            json);

        return Ok(new
        {
            success = true,
            message = "Stalwart webhook received."
        });
    }
}
