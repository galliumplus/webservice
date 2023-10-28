namespace GalliumPlus.WebApi.Core.Email
{
    public interface IEmailTemplateLoader
    {
        EmailTemplate LoadTemplate(string identifier);
    }
}