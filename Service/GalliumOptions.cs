#if !TEST
using GalliumPlus.WebApi.Email.MailKit;
using GalliumPlus.WebApi.Data.MariaDb;
#endif

namespace GalliumPlus.WebApi
{
    public class GalliumOptions
    {
        public bool ListenAnyIp { get; set; } = false;

        public string? CertificateFile { get; set; } = null;

        public string? CertificatePassword { get; set; } = null;

        public int HttpPort { get; set; } = 5080;

        public int HttpsPort { get; set; } = 5443;

        public bool ForceHttps { get; set; } = true;

        public bool DisableHttps { get; set; } = false;

        public string Host { get; set; } = "localhost";
        
        public string PreferredWebApplicationHost { get; set; } = "gallium-plus-preview.netlify.app";

#if !TEST
        public MailKitOptions MailKit { get; set; } = new();

        public MariaDbOptions MariaDb { get; set; } = new();
#endif
    }
}
