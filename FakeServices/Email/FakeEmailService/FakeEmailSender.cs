using GalliumPlus.WebApi.Core.Email;
using System.Text;

namespace GalliumPlus.WebApi.Email.FakeEmailService
{
    public class FakeEmailSender : IEmailSender
    {
        private static string Repeat(string text, int times)
        {
            StringBuilder sb = new();
            for (int i = 0; i < times; i++) sb.Append(text);
            return sb.ToString();
        }

        public void Send(string recipient, string subject, string content)
        {
            int width = Console.WindowWidth;

            Console.WriteLine("== Envoi de mail ==" + Repeat("=", width - 19));
            Console.WriteLine();
            Console.WriteLine("   De : Gallium+ <no-reply@etiq-dijon.fr>");
            Console.WriteLine("   À : {0}", recipient);
            Console.WriteLine("   Sujet : {0}", subject);
            Console.WriteLine();
            Console.Write(content);
            Console.WriteLine();
            Console.WriteLine(Repeat("=", width));
        }

        public Task SendAsync(string recipient, string subject, string content, CancellationToken ct = default)
        {
            this.Send(recipient, subject, content);
            return Task.CompletedTask;
        }
    }
}
