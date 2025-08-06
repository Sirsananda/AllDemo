using Microsoft.AspNetCore.DataProtection;

namespace AllDemo.Helper.Validator
{
    public class GoogleReCaptchaValidator
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleReCaptchaValidator(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> IsReCaptchaPassedAsync(string token)
        {
            var secretKey = _configuration["GoogleReCaptcha:SecretKey"];
            var url= $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}";
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(url, null);
            var json = await response.Content.ReadAsStringAsync();

            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            return result.success == true;
        }
    }
}
