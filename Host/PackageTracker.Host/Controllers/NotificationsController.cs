using MediatR;
using Microsoft.AspNetCore.Mvc;
using PackageTracker.Domain.Notifications.Exceptions;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Queries;

namespace PackageTracker.WebHost.Controllers;

public class NotificationsController : Controller
{
    private readonly IMediator mediator;

    public NotificationsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var queryResponse = await mediator.Send(new GetLatestNotificationsQuery());
        return View(queryResponse.Notifications);
    }

    public async Task<IActionResult> Unread()
    {
        var queryResponse = await mediator.Send(new GetUnreadNotificationsQuery());
        return Json(queryResponse.Notifications);
    }

    [HttpPut]
    public async Task<IActionResult> Read(Guid id)
    {
        try
        {
            await mediator.Send(new ReadNotificationCommand { NotificationId = id });
            return Ok();
        }
        catch (NotificationNotFoundException)
        {
            return NotFound();
        }
    }
}
