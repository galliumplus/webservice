namespace GalliumPlusAPI.Database
{
    public interface IMasterDao
    {
        public IProductDao Products { get; }

        public IRoleDao Roles { get; }
    }
}
