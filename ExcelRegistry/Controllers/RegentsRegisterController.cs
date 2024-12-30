using ExcelRegistry.Domain.Handlers.Regents;
using ExcelRegistry.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExcelRegistryApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class RegentsRegisterController(IMediator mediator) : ControllerBase
{
    [HttpPost(nameof(AddRegent))]
    [Authorize(Roles = $"{Constants.Roles.Admin},{Constants.Roles.User}")]
    public async Task AddRegent(AddRegentRequest request)
    {
        await mediator.Send(request);
    }

    [HttpGet(nameof(GetRegents))]
    [Authorize(Roles = $"{Constants.Roles.Admin},{Constants.Roles.User}")]
    public async Task<IEnumerable<GetRegentResponseItem>> GetRegents()
    {
        return await mediator.Send(new GetRegentsRequest());
    }

    [HttpPatch(nameof(EditRegent))]
    [Authorize(Roles = $"{Constants.Roles.Admin},{Constants.Roles.User}")]
    public async Task EditRegent(EditRegentRequest request)
    {
        await mediator.Send(request);
    }
}
