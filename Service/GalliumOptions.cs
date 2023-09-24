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

        public class MariaDbOptions
        {
            public string Host { get; set; }

            public string UserId { get; set; }

            public string Password { get; set; }

            public string Schema { get; set; }

            public uint Port { get; set; }

            public MariaDbOptions()
            {
                this.Host = "localhost";
                this.UserId = "";
                this.Password = "";
                this.Schema = "galliumplus";
                this.Port = 3306;
            }
        }

        public MariaDbOptions MariaDb { get; set; }

        public GalliumOptions()
        {
            ListenAnyIp = false;
            CertificateFile = null;
            CertificatePassword = null;
            HttpPort = 5080;
            HttpsPort = 5443;
            ForceHttps = true;
            MariaDb = new();
        }
    }
}
