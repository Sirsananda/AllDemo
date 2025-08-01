using AllDemo.Helper.Log;
using AllDemo.Services;
using AllDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllDemo.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class EmailController : Controller
    {
        private readonly EmailService _emailService;
        private readonly ErrorLog _errorLog;
        public EmailController(EmailService emailService,ErrorLog errorLog)
        {
            _emailService = emailService;
            _errorLog = errorLog;
        }
        public IActionResult Index()
        {
            EmailFormViewModel model = new EmailFormViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailToUser(EmailFormViewModel emailFormModel)
        {
            try
            {
                List<string> arr = new List<string>();
                if (!ModelState.IsValid)
                {
                    return View("Index", emailFormModel);
                }
                bool result = await _emailService.SendEmail(emailFormModel.Subject, emailFormModel.BodyMessage, emailFormModel.SingleFile, emailFormModel.MultipleFile);
                if (result)
                {
                    TempData["success"] = "Email send to the registered email address";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["failed"] = "Email send failed!";
                    return View("Index", emailFormModel);
                }
            }
            catch (Exception ex)
            {
                _errorLog.WriteErrorLog(ex, "Error generate at SendEmailToUser()");
                TempData["failed"] = "Email send failed!";
                return RedirectToAction("Index", emailFormModel);
            }
        }
    }
}
