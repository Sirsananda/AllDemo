using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace AllDemo.Helper.Log
{
    public  class ErrorLog
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        public  ErrorLog(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }
        public  void WriteErrorLog(Exception ex, string message = "")
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            string currentPath = _httpContextAccessor.HttpContext?.Request.Path;
            string errorLogFileName = "ErrorLog_" + DateTime.Now.ToString("dd-MM-yyyy", cultureInfo) + ".txt";
            string rootPath = _env.ContentRootPath;//here i get the root path of the application
            string path = Path.Combine(rootPath, "ErrorLogs", errorLogFileName);
            string directoryPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            //using StringBuilder to concate the Error message details
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("-------------------------ErrorLog Start------------------- as on " + DateTime.Now.ToString("T", cultureInfo));
            stringBuilder.AppendLine("Webpage Name: " + currentPath);
            stringBuilder.AppendLine("Custom Message :" + message);
            stringBuilder.AppendLine("Message :" + ex.Message);
            stringBuilder.AppendLine("ErrorLog Details: " + ex.StackTrace);
            stringBuilder.AppendLine("-------------------------ErrorLog End-------------------");

            //write error to the file
            if (File.Exists(path))
            {
                using (StreamWriter streamWriter = new StreamWriter(path, true))
                {
                    streamWriter.WriteLine(stringBuilder.ToString());
                }
            }
            else
            {
                using (StreamWriter streamWriter = new StreamWriter(path))
                {
                    streamWriter.WriteLine(stringBuilder.ToString());
                }
            }
        }
    }
}
