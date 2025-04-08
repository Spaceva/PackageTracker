using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PackageTracker.Domain.Modules;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Queries;
using PackageTracker.Presentation.MVCApp.Mappers;
using PackageTracker.Presentation.MVCApp.Models;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Presentation.MVCApp.Controllers;

public class ModuleController(IMediator mediator, IMapper mapper) : Controller
{
    public async Task<IActionResult> Index()
    {
        var queryResponse = await mediator.Query<GetModulesQuery, GetModulesQueryResponse>(new GetModulesQuery());
        return View(mapper.MapCollection<Module, ModuleViewModel>(queryResponse.Modules));
    }
    public async Task<IActionResult> Toggle([FromBody] ToggleModuleCommand command)
    {
        try
        {
            await mediator.Send(command);
            return Ok();
        }
        catch (ModuleNotFoundException)
        {
            return NotFound();
        }
    }
}
