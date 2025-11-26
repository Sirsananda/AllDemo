using Microsoft.AspNetCore.Mvc;

namespace AllDemo.Controllers
{
    public class DepartmentController : Controller
    {
        public IActionResult DepartmentList()
        {
            return View();
        }
    }
}
