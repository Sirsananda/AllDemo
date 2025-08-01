namespace AllDemo.Configuration
{
    public class MailSetting
    {
        public string SMTPFrom { get; set; }
        public string SMTPTo { get; set; }
        public string SMTPCc { get; set; }
        public string SMTPHost { get; set; }
        public int SMTPPort { get; set; }
        public string SMTPUser { get; set; }
        public string SMTPPassword { get; set; }
        public bool SMTPUseSSL { get; set; }
        public bool SMTPAuthentication { get; set; }
    }
}
