using Quartz;
using System.Reflection;

namespace GalliumPlus.WebApi.Scheduling
{
    /// <summary>
    /// Indique que le job doit être ajouté au conteneur d'injection de dépendances avec cette configuration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class JobConfigurationAttribute : Attribute
    {
        /// <summary>
        /// Le nom du job.
        /// </summary>
        public string Name { get; private init; }

        /// <summary>
        /// Le groupe du job.
        /// </summary>
        public string? Group { get; init; }

        /// <summary>
        /// Mettre à <see langword="true"/> pour que le job soit persistant.
        /// </summary>
        public bool StoreDurably { get; init; } = false;

        /// <summary>
        /// Mettre à <see langword="true"/> pour relancer le job quand il échoue.
        /// </summary>
        public bool ShouldRecover { get; init; } = false;

        /// <param name="name">Le nom du job.</param>
        public JobConfigurationAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Configure le job associé.
        /// </summary>
        public void ConfigureJob(IJobConfigurator configurator)
        {
            if (this.Group is null)
            {
                configurator.WithIdentity(this.Name);
            }
            else
            {
                configurator.WithIdentity(this.Name, this.Group);
            }

            configurator.StoreDurably(this.StoreDurably);
            configurator.RequestRecovery(this.ShouldRecover);
        }
    }


    public static class JobConfigurationExtensions
    {
        /// <summary>
        /// Ajoute tous les jobs avec un attribut [JobConfiguration].
        /// </summary>
        /// <returns>Le <see cref="IServiceCollectionQuartzConfigurator"/> avec les nouveaux jobs.</returns>
        public static IServiceCollectionQuartzConfigurator AddJobs(this IServiceCollectionQuartzConfigurator @this)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach ((Type selfConfiguringJob, object[] attributes)
                in assembly.GetTypes()
                           .Select(type => (type, type.GetCustomAttributes(typeof(JobConfigurationAttribute), false)))
                           .Where(typeAndAttributes => typeAndAttributes.Item2.Length > 0)
            )
            {
                var attribute = (JobConfigurationAttribute)attributes.First();
                @this.AddJob(selfConfiguringJob, null, attribute.ConfigureJob);
            }

            return @this;
        }
    }
}
