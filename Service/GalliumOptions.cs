namespace GalliumPlus.WebApi
{
    public class GalliumOptions
    {
        public bool ListenAnyIp { get; set; }

        public string? CertificateFile { get; set; }

        public string? CertificatePassword { get; set; }

        public int HttpPort { get; set; }

        public int HttpsPort { get; set; }

        public bool ForceHttps { get; set; }

        public GalliumOptions()
        {
            ListenAnyIp = false;
            CertificateFile = null;
            CertificatePassword = null;
            HttpPort = 5080;
            HttpsPort = 5443;
            ForceHttps = true;
        }
    }
}
