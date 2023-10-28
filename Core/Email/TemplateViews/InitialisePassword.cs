using Humanizer;
using System.Globalization;

namespace GalliumPlus.WebApi.Core.Email.TemplateViews
{
    public class InitialisePassword
    {
        public string Link { get; private init; }

        public DateTime Expiration { get; private init; }

        public string HumanExpiration => this.Expiration.Humanize(utcDate: true, culture: new CultureInfo("fr-FR"));

        public InitialisePassword(string link, DateTime expiration)
        {
            this.Link = link;
            this.Expiration = expiration;
        }
    }
}
