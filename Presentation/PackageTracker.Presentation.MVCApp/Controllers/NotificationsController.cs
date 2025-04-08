using Microsoft.AspNetCore.Mvc;
using PackageTracker.Domain.Notifications.Exceptions;
using PackageTracker.Domain.Notifications.Model;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Queries;
using PackageTracker.Presentation.MVCApp.Models;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Presentation.MVCApp.Controllers;

public class NotificationsController(IMediator mediator) : Controller
{
    public async Task<IActionResult> Index()
    {
        var queryResponse = await mediator.Query<GetLatestNotificationsQuery, GetLatestNotificationsQueryResponse>(new GetLatestNotificationsQuery());
        return View(queryResponse.Notifications);
    }

    public async Task<IActionResult> Unread()
    {
        var queryResponse = await mediator.Query<GetUnreadNotificationsQuery, GetUnreadNotificationsQueryResponse>(new GetUnreadNotificationsQuery());
        return Json(queryResponse.Notifications);
    }

    [HttpPost]
    public IActionResult Toast([FromBody] Notification notification)
    {
        return PartialView("_NotificationToast", new NotificiationToastViewModel(notification));
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

    [HttpPost]
    public async Task<IActionResult> ReadAll()
    {
        await mediator.Send(new ReadAllNotificationsCommand());
        return Ok();
    }
}
