using Stubble.Core.Interfaces;
using System.Collections.Concurrent;

namespace GalliumPlus.WebApi.Core.Email
{
    public class CachedLocalEmailTemplateLoader : IEmailTemplateLoader, IStubbleLoader
    {
        private string baseDirectory;
        private Dictionary<string, string> cache;

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
                    using (StreamReader f = new(Path.Join(this.baseDirectory, name)))
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

        public EmailTemplate LoadTemplate(string identifier)
        {
            return new EmailTemplate(this.Load(identifier) ?? "", this);
        }
    }
}
