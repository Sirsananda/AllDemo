using Microsoft.AspNetCore.Mvc;

namespace AllDemo.Controllers
{
    public class CKEditorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CopyText()
        {
            return View();
        }
    }
}
