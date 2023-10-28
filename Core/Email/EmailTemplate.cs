using Stubble.Core;
using Stubble.Core.Builders;
using Stubble.Core.Interfaces;

namespace GalliumPlus.WebApi.Core.Email
{
    /// <summary>
    /// Un modèle de mail.
    /// </summary>
    public class EmailTemplate
    {
        private string raw;
        private StubbleVisitorRenderer renderer;

        /// <summary>
        /// Crée un nouveau modèle de mail.
        /// </summary>
        /// <param name="rawTemplate">Le contenu du modèle.</param>
        /// <param name="partialTemplateLoader">(optionnel) Le chargeur de modèles partiels.</param>
        public EmailTemplate(string rawTemplate, IStubbleLoader? partialTemplateLoader)
        {
            this.raw = rawTemplate;
            
            this.renderer = new StubbleBuilder()
                .Configure(settings =>
                {
                    if (partialTemplateLoader is not null)
                    {
                        settings.AddToTemplateLoader(partialTemplateLoader);
                    }
                })
                .Build();
        }

        /// <summary>
        /// Imprime un corps de mail à partir des données fournies.
        /// </summary>
        /// <param name="view">Les données à utiliser pour remplir le modèle.</param>
        /// <returns>Le corps du mail.</returns>
        public string Render(object view)
        {
            return this.renderer.Render(this.raw, view);
        }

        /// <inheritdoc cref="Render(object)"/>
        public ValueTask<string> RenderAsync(object view)
        {
            return renderer.RenderAsync(this.raw, view);
        }
    }
}
