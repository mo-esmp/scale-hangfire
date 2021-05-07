using Microsoft.AspNetCore.Mvc;

namespace JobQueue.ConsumerService.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("~/hangfire");
        }
    }
}