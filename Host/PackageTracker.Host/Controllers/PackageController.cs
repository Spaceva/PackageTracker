using Microsoft.AspNetCore.Mvc;
using PackageTracker.Domain.Packages.Exceptions;
using MediatR;
using PackageTracker.Messages.Queries;

namespace PackageTracker.WebHost.Controllers;

public class PackageController : Controller
{
    private readonly IMediator mediator;

    public PackageController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<IActionResult> Details(string id)
    {
        try
        {
            var queryResponse = await mediator.Send(new GetPackageByNameQuery { PackageName = Uri.UnescapeDataString(id) });
            return View(queryResponse!);
        }
        catch (PackageNotFoundException)
        {
            return View("PackageNotFound");
        }
    }
}
