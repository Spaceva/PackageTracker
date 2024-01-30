using Microsoft.AspNetCore.Mvc;
using PackageTracker.Domain.Package.Exceptions;
using MediatR;
using PackageTracker.Messages.Queries;
using PackageTracker.Messages.Commands;
using PackageTracker.Presentation.MVCApp.Models;
using AutoMapper;
using PackageTracker.Presentation.MVCApp.Mappers;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Presentation.MVCApp.Controllers;

public class PackageController(IMediator mediator, IMapper mapper) : Controller
{
    public async Task<IActionResult> Index()
    {
        var queryResponse = await mediator.Send(new GetPackagesQuery() { SearchCriteria = new Domain.Package.PackageSearchCriteria { } });
        if (queryResponse is null)
        {
            return View(Array.Empty<PackageViewModel>());
        }

        return View(mapper.MapCollection<Package, PackageViewModel>(queryResponse.Packages));
    }

    public async Task<IActionResult> Details(string id)
    {
        try
        {
            var queryResponse = await mediator.Send(new GetPackagesQuery { SearchCriteria = new Domain.Package.PackageSearchCriteria { Name = Uri.UnescapeDataString(id) } });
            return View(mapper.Map<Package, PackageWithVersionsViewModel>(queryResponse.Packages.Single()));
        }
        catch (PackageNotFoundException)
        {
            return View("PackageNotFound");
        }
    }

    public async Task<IActionResult> Delete([FromBody] DeletePackageCommand command)
    {
        try
        {
            await mediator.Send(command);
            return Ok();
        }
        catch (PackageNotFoundException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> TrackPackage([FromBody] TrackPackageViewModel trackPackageViewModel)
    {
        await mediator.Send(new FetchPackageCommand(trackPackageViewModel.PackageName, trackPackageViewModel.PackageType));
        return Ok();
    }
}
