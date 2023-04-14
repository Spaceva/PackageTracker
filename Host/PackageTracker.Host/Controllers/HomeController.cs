using Microsoft.AspNetCore.Mvc;
using MediatR;
using PackageTracker.Messages.Queries;
using PackageTracker.WebHost.Models;
using PackageTracker.Domain.Notifications.Model;

namespace PackageTracker.WebHost.Controllers;

public class HomeController : Controller
{
    private readonly IMediator mediator;

    public HomeController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var queryResponse = await mediator.Send(new GetAllPackagesQuery());
        if (queryResponse is null)
        {
            return View(Array.Empty<GetAllPackagesQueryResponse.PackageDto>());
        }

        return View(queryResponse.Packages);
    }

    [HttpPost("toast")]
    public IActionResult Toast([FromBody] Notification notification)
    {
        return PartialView("_NotificationToast", new NotificiationToastViewModel(notification));
    }
}
