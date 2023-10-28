using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Email
{
    public interface IEmailSender
    {
        void Send(string recipient, string subject, string content);
    }
}
