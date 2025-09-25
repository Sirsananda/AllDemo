using AllDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace AllDemo.Controllers
{
    public class ChatBotController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("SendMessage")]
        public IActionResult SendMessage(ChatRequestViewModel request)
        {
            string botResponse = GetBotResponse(request.Message);

            return Ok(new
            {
                User = request.User,
                UserMessage = request.Message,
                Bot = "ChatBot 🤖",
                BotMessage = botResponse
            });
        }

        private string GetBotResponse(string requestMessage)
        {
            requestMessage = requestMessage.ToLower();

            if (requestMessage.Contains("hello"))
                return "Hi there! How can I help you today?";
            else if (requestMessage.Contains("time"))
                return $"Current server time: {DateTime.Now}";
            else if (requestMessage.Contains("bye"))
                return "Goodbye! Have a great day 😊";
            else
                return "I'm not sure how to answer that yet.";
        }
    }
}
