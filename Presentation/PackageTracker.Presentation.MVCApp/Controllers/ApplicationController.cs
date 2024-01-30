using Microsoft.AspNetCore.Mvc;
using MediatR;
using PackageTracker.Messages.Queries;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Application;
using System.Text;
using PackageTracker.Presentation.MVCApp.Modules;
using PackageTracker.Messages.Commands;
using PackageTracker.Domain.Application.Exceptions;
using PackageTracker.Presentation.MVCApp.Models;
using AutoMapper;
using PackageTracker.Presentation.MVCApp.Mappers;

namespace PackageTracker.Presentation.MVCApp.Controllers;

public class ApplicationController(IMediator mediator, IMapper mapper) : Controller
{
    public IActionResult Index()
    {
        return View(Array.Empty<ApplicationDetailViewModel>());
    }

    public async Task<IActionResult> Angular()
    {
        var queryResponse = await mediator.Send(new GetApplicationsQuery { SearchCriteria = new() { ApplicationTypes = new[] { ApplicationType.Angular } } });
        return View("Index", mapper.MapCollection<Application, ApplicationDetailViewModel>(queryResponse.Applications));
    }

    public async Task<IActionResult> DotNet()
    {
        var queryResponse = await mediator.Send(new GetApplicationsQuery { SearchCriteria = new() { ApplicationTypes = new[] { ApplicationType.DotNet } } });
        return View("Index", mapper.MapCollection<Application, ApplicationDetailViewModel>(queryResponse.Applications));
    }

    public async Task<IActionResult> PHP()
    {
        var queryResponse = await mediator.Send(new GetApplicationsQuery { SearchCriteria = new() { ApplicationTypes = new[] { ApplicationType.Php } } });
        return View("Index", mapper.MapCollection<Application, ApplicationDetailViewModel>(queryResponse.Applications));
    }

    public async Task<IActionResult> Gitlab()
    {
        var queryResponse = await mediator.Send(new GetApplicationsQuery { SearchCriteria = new() { RepositoryTypes = new[] { RepositoryType.Gitlab } } });
        return View("Index", mapper.MapCollection<Application, ApplicationDetailViewModel>(queryResponse.Applications));
    }

    public async Task<IActionResult> AzureDevOps()
    {
        var queryResponse = await mediator.Send(new GetApplicationsQuery { SearchCriteria = new() { RepositoryTypes = new[] { RepositoryType.AzureDevOps } } });
        return View("Index", mapper.MapCollection<Application, ApplicationDetailViewModel>(queryResponse.Applications));
    }

    public async Task<IActionResult> Search(ApplicationSearchCriteria searchCriteria)
    {
        var queryResponse = await mediator.Send(new GetApplicationsQuery { SearchCriteria = searchCriteria });
        return View("Index", mapper.MapCollection<Application, ApplicationDetailViewModel>(queryResponse.Applications));
    }

    [HttpPost]
    public async Task<IActionResult> Delete([FromBody] ApplicationViewModel application)
    {
        try
        {
            await mediator.Send(new DeleteApplicationCommand { Name = application.Name, RepositoryLink = application.RepositoryLink, Type = Enum.Parse<ApplicationType>(application.Type) });
            return Ok();
        }
        catch (ApplicationNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> MarkDecommissionned([FromBody] ApplicationViewModel applicationViewModel)
    {
        try
        {
            var queryResponse = await mediator.Send(new GetApplicationQuery { Name = applicationViewModel.Name, RepositoryLink = applicationViewModel.RepositoryLink, Type = Enum.Parse<ApplicationType>(applicationViewModel.Type) });
            var application = queryResponse.Application;
            application.IsSoonDecommissioned = true;
            await mediator.Send(new UpdateApplicationCommand { Application = application });
            return Ok();
        }
        catch (ApplicationNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> UnmarkDecommissionned([FromBody] ApplicationViewModel applicationViewModel)
    {
        try
        {
            var queryResponse = await mediator.Send(new GetApplicationQuery { Name = applicationViewModel.Name, RepositoryLink = applicationViewModel.RepositoryLink, Type = Enum.Parse<ApplicationType>(applicationViewModel.Type) });
            var application = queryResponse.Application;
            application.IsSoonDecommissioned = false;
            await mediator.Send(new UpdateApplicationCommand { Application = application });
            return Ok();
        }
        catch (ApplicationNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> UnmarkDeadLink([FromBody] ApplicationViewModel applicationViewModel)
    {
        try
        {
            var queryResponse = await mediator.Send(new GetApplicationQuery { Name = applicationViewModel.Name, RepositoryLink = applicationViewModel.RepositoryLink, Type = Enum.Parse<ApplicationType>(applicationViewModel.Type) });
            var application = queryResponse.Application;
            application.IsDeadLink = false;
            await mediator.Send(new UpdateApplicationCommand { Application = application });
            return Ok();
        }
        catch (ApplicationNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public IActionResult CSV([FromBody] IReadOnlyCollection<ApplicationDetailViewModel> rows)
    {
        return File(Encoding.UTF8.GetBytes(CsvExporter.Export(rows)), "APPLICATION/OCTET-STREAM", "applications.csv");
    }
}
