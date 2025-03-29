using Microsoft.AspNetCore.Mvc;

public class TestController : Controller
{
    public IActionResult Index()
    {
        // Pass a message to the view to test dynamic content
        ViewData["Message"] = "Hello, this is the Test Page!";
        return View();
    }
}
