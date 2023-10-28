using Stubble.Core;
using Stubble.Core.Builders;
using Stubble.Core.Interfaces;

namespace GalliumPlus.WebApi.Core.Email
{
    public class EmailTemplate
    {
        private string raw;
        private StubbleVisitorRenderer renderer;

        public EmailTemplate(string rawTemplate, IStubbleLoader? templateLoader)
        {
            this.raw = rawTemplate;
            
            this.renderer = new StubbleBuilder()
                .Configure(settings =>
                {
                    if (templateLoader is not null)
                    {
                        settings.AddToTemplateLoader(templateLoader);
                    }
                })
                .Build();
        }

        public string Render(object view)
        {
            return this.renderer.Render(this.raw, view);
        }

        public ValueTask<string> RenderAsync(object view)
        {
            return renderer.RenderAsync(this.raw, view);
        }
    }
}
