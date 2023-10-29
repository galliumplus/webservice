using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Email.MailKit
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
