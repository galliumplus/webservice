using GalliumPlus.Core.Security;

namespace GalliumPlus.WebService.Middleware.Authorization
{
    /// <summary>
    /// Indique les permissions requises pour effectuer une action de contrôleur.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RequiresPermissionsAttribute : Attribute
    {
        private Permission required;

        /// <summary>
        /// Les permissions requises.
        /// </summary>
        public Permission Required => this.required;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="required"></param>
        public RequiresPermissionsAttribute(Permission required)
        {
            this.required = required;
        }
    }
}
