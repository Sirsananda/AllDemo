using AllDemo.Configuration;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;

namespace AllDemo.Services
{
    public class OTPService
    {
        private readonly int _otpLength;
        public OTPService(IOptions<OTPSetting> options)
        {
            _otpLength = options.Value.Length;
        }

        public string GenerateOTP()
        {
            var random=new Random();
            var OTP = random.Next((int)Math.Pow(10, _otpLength - 1), (int)Math.Pow(10, _otpLength)).ToString();
            return OTP;
        }
    }
}
