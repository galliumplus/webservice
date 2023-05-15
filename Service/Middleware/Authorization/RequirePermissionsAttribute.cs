namespace GalliumPlus.WebApi.Middleware.Authorization
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RequirePermissionsAttribute : Attribute
    {
        public RequirePermissionsAttribute() => throw new NotImplementedException();
    }
}
