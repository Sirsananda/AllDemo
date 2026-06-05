using Microsoft.AspNetCore.Mvc;

namespace AllDemo.Controllers
{
    public class VideoTranscoderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>  Upload(IFormFile file)
        {
            return RedirectToAction("Index");
        }
    }
}
