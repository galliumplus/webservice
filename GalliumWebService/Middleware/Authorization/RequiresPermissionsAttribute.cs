using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Middleware.Authorization
{
    /// <summary>
    /// Indique les permissions requises pour effectuer une action de contrôleur.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RequiresPermissionsAttribute : Attribute
    {
        private Permissions required;

        /// <summary>
        /// Les permissions requises.
        /// </summary>
        public Permissions Required => this.required;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="required"></param>
        public RequiresPermissionsAttribute(Permissions required)
        {
            this.required = required;
        }
    }
}
