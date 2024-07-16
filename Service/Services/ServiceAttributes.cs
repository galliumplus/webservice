using System.Reflection;
using GalliumPlus.WebApi.Scheduling;
using Quartz;

namespace GalliumPlus.WebApi.Services
{
    /// <summary>
    /// Indique que cette classe doit être enregistrée dans le conteneur d'injection de dépendances.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ScopedServiceAttribute : Attribute;

    public static class ServiceExtensions
    {
        /// <summary>
        /// Ajoute toutes les classes avec un attribut [ScopedService].
        /// </summary>
        public static IServiceCollection AddGalliumServices(this IServiceCollection @this)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (Type service in assembly.GetTypes())
            {
                if (service.GetCustomAttributes<ScopedServiceAttribute>(false).Any())
                {
                    @this.AddScoped(service);
                }
            }

            return @this;
        }
    }
}
