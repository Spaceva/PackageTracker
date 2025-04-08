using Microsoft.AspNetCore.Mvc;
using PackageTracker.Messages.Queries;
using PackageTracker.Messages.Commands;
using PackageTracker.Domain.Framework.Exceptions;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Domain.Framework;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Presentation.MVCApp.Controllers;

public class FrameworkController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var queryResponse = await mediator.Query<GetFrameworksQuery, GetFrameworksQueryResponse>(new GetFrameworksQuery { SearchCriteria = new FrameworkSearchCriteria { Status = [FrameworkStatus.Active, FrameworkStatus.LongTermSupport] } });
        return View(queryResponse!);
    }

    [HttpGet]
    public async Task<IActionResult> All()
    {
        var queryResponse = await mediator.Query<GetFrameworksQuery, GetFrameworksQueryResponse>(new GetFrameworksQuery { SearchCriteria = new FrameworkSearchCriteria() });
        return View(nameof(Index), queryResponse!);
    }

    [HttpPost]
    public async Task<IActionResult> Delete([FromBody] DeleteFrameworkCommand command)
    {
        try
        {
            await mediator.Send(command);
            return Ok();
        }
        catch (FrameworkNotFoundException)
        {
            return NotFound();
        }
    }
}
