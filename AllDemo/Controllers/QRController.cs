using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing.Imaging;

namespace AllDemo.Controllers
{
    public class QRController : Controller
    {
        public IActionResult SimpleQRCode()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Generate(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Json(new { success = false, message = "Failed to create QR Code" });
            }
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

            var base64QrCode = new Base64QRCode(qrCodeData);
            var qrCodeImageAsBase64 = base64QrCode.GetGraphic(10);

            return Json(new { success = true, image = "data:image/png;base64," + qrCodeImageAsBase64 });
        }
    }
}
