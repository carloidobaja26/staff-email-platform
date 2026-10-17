using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using Microsoft.AspNetCore.Mvc;
using EmailPlatform.Application.Common.Pagination;
using EmailPlatformApi.Api.Controllers;

namespace EmailPlatformApi.Controllers;

[ApiController]
[Route("api/emails")]
public class EmailsController : BaseController
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

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] PaginationRequest request)
    {
        var tenantId = Guid.Parse("a8a87514-8fa1-468d-a58a-5a29117997ac");

        return HandleResponse(
            await _emailService.GetPagedAsync(
                tenantId,
                request));
    }

    [HttpGet("AdminPaged")]
    public async Task<IActionResult> GetAdminPaged(
        [FromQuery] PaginationRequest request)
    {
        return HandleResponse(
            await _emailService.GetAdminPagedAsync(
                request));
    }

   [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        return HandleResponse(await _emailService.GetByIdAsync(id));
    }
}
