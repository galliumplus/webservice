using Stubble.Core.Interfaces;
using System.Collections.Concurrent;
using System.Text;

namespace GalliumPlus.WebApi.Core.Email
{
    /// <summary>
    /// Un chargeur de modèles qui va récupère les données depuis le système de
    /// fichiers local et les mets en cache. Les modèles sont identifiés par leur
    /// nom de fichier.
    /// </summary>
    public class CachedLocalEmailTemplateLoader : IEmailTemplateLoader, IStubbleLoader
    {
        private string baseDirectory;
        private Dictionary<string, string> cache;

        /// <summary>
        /// Crée un nouveau chargeur local.
        /// </summary>
        /// <param name="baseDirectory">Le chemin absolu du dossier contenant les modèles.</param>
        public CachedLocalEmailTemplateLoader(string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
            this.cache = new();
        }

        public IStubbleLoader Clone()
        {
            var loader = new CachedLocalEmailTemplateLoader(this.baseDirectory)
            {
                cache = new(this.cache)
            };
            return loader;
        }

        public string Load(string name)
        {
            string? template;
            bool cached;

            lock (this.cache)
            {
                cached = this.cache.TryGetValue(name, out template);
            }

            if (!cached)
            {
                try
                {
                    using (StreamReader f = new(Path.Join(this.baseDirectory, name + ".html")))
                    {
                        template = f.ReadToEnd();
                    }
                    lock (this.cache)
                    {
                        cache.TryAdd(name, template);
                    }
                }
                catch (FileNotFoundException err)
                {
                    Console.WriteLine(err.Message);
                    template = null;
                }
            }

            return template!;
        }

        public async ValueTask<string> LoadAsync(string name)
        {
            string? template;

            if (!cache.TryGetValue(name, out template))
            {
                try
                {
                    using (StreamReader f = new(Path.Join(this.baseDirectory, name), Encoding.UTF8))
                    {
                        template = await f.ReadToEndAsync();
                    }
                    cache.Add(name, template);
                }
                catch (FileNotFoundException err)
                {
                    Console.WriteLine(err.Message);
                    template = null;
                }
            }

            return template!;
        }

        public EmailTemplate LoadTemplate(string identifier, bool ignoreNullContent = true)
        {
            if (this.Load(identifier) is string template)
            {
                return new EmailTemplate(template, this);
            }
            else if (ignoreNullContent)
            {
                return new EmailTemplate("", this);
            }
            else
            {
                throw new FileNotFoundException("Impossible de charger un modèle local", identifier);
            }
        }
    }
}
