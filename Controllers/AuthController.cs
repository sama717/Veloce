using Microsoft.AspNetCore.Mvc;

namespace Veloce.Controllers;

public class AuthController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}