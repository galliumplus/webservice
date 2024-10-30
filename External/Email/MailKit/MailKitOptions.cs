namespace GalliumPlus.Email.MailKit
{
    public class MailKitOptions
    {
        public string SenderDisplayName { get; set; } = "";

        public string SenderAddress { get; set; } = "";

        public bool UseSSL { get; set; } = false;

        public bool UseStartTls { get; set; } = false;

        public string Host { get; set; } = "localhost";

        public int Port { get; set; } = 25;

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";
    }
}
