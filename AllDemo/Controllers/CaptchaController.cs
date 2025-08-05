using Microsoft.AspNetCore.Mvc;


namespace AllDemo.Controllers
{
    public class CaptchaController : Controller
    {
        private readonly Random _random = new();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GenerateCaptcha()
        {
            int num1 = _random.Next(10, 100);
            int num2 = _random.Next(1, num1); //confirm num1>num2
            string captchaText = $"{num1}-{num2}";
            int answer = num1 - num2;
            HttpContext.Session.SetInt32("CaptchaAnswer", answer);
            return Json(new { captcha = captchaText });
        }

        [HttpPost]
        public JsonResult ValidateRecaptcha(string input)
        {
            int? correctAnswer = HttpContext.Session.GetInt32("CaptchaAnswer");

            return Json(new { success = Convert.ToInt32(input)  == correctAnswer ? true : false });

        }

    }

}
