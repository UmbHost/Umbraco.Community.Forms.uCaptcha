namespace Umbraco.Community.Forms.uCaptcha.Models
{
    public class uCaptchaSettings
    {
        public string SiteKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;

        public string Provider { get; set; } = string.Empty;
    }
}