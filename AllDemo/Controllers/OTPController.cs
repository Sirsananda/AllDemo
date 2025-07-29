using AllDemo.Helper.Log;
using AllDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllDemo.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class OTPController : Controller
    {
       
        private readonly OTPService _otpService;
        public OTPController(OTPService otpService)
        {
            this._otpService = otpService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOTP()
        {
            string otp = _otpService.GenerateOTP();
            TempData["OTP"] = "Your OTP is:" + otp;
            return RedirectToAction("Index");
        }
    }
}
