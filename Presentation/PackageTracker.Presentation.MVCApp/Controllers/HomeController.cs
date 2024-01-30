using Microsoft.AspNetCore.Mvc;
using PackageTracker.Presentation.MVCApp.Models;

namespace PackageTracker.Presentation.MVCApp.Controllers;

public class HomeController() : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction(nameof(PackageController.Index), nameof(PackageController).Replace("Controller", string.Empty));
    }

    [HttpPost]
    public IActionResult Toast([FromBody] BasicToastViewModel viewModel)
    {
        return PartialView("_BasicToast", viewModel);
    }
}
