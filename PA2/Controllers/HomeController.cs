using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PA2.Models;

namespace PA2.Controllers;

//main controller for home related requests
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logService;

    //putting the logger into controller
    public HomeController(ILogger<HomeController> logService)
    {
        _logService = logService;
    }

    //removed the "homepage" and "privacy" page functionality and options to make the code easiuer to understand and read
    /**public IActionResult Index()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }
    **/

    //error page generation
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var errorModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
        return View(errorModel);
    }
}
