using System.Drawing;
using System.Drawing.Imaging;
using AllDemo.Helper.Validator;
using Microsoft.AspNetCore.Mvc;


namespace AllDemo.Controllers
{
    public class CaptchaController : Controller
    {
        private readonly GoogleReCaptchaValidator _googleReCaptchaValidator;

        public CaptchaController(GoogleReCaptchaValidator googleReCaptchaValidator)
        {
            _googleReCaptchaValidator = googleReCaptchaValidator;
        }
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

        [HttpGet]
        public IActionResult ImageReCAPTCHA()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GenerateImage()
        {
            int length = _random.Next(4, 9);
            string captchatext = GenerateRandomText(length);
            HttpContext.Session.SetString("ImageCaptchaCode",captchatext);
            var stream = new MemoryStream();
            using (var bmp = new Bitmap(160, 40)) 
            using (var gfx = Graphics.FromImage(bmp))
            using (var font = new Font("Arial", 20, FontStyle.Bold))
            {
                gfx.Clear(Color.Navy);
                gfx.DrawString(captchatext,font,Brushes.White,new PointF(10,5));
                bmp.Save(stream,ImageFormat.Png);
            }

            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "image/png");

        }

        private string GenerateRandomText(int length)
        {
            const string chars= "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        public JsonResult ValidateImageCaptcha(string input)
        {
            string correctCaptcha = HttpContext.Session.GetString("ImageCaptchaCode");
            return Json(new { success = input == correctCaptcha });
        }

        [HttpGet]
        public IActionResult reCAPTCHAVTWO()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> reCAPTCHAVTWO(string username)
        {
            string token = Request.Form["g-recaptcha-response"];
            bool isHuman = await _googleReCaptchaValidator.IsReCaptchaPassedAsync(token);

            if (!isHuman)
            {
                ModelState.AddModelError("", "Captcha failed. Please try again.");
                return View(); // Return form again
            }

            TempData["reCAPTCHA-v2"] = "CAPTCHA Passed!";
            // Proceed with form logic
            return View();
        }
    }

}
