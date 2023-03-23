namespace GalliumPlusAPI.Database
{
    public interface IMasterDao
    {
        public IBundleDao Bundles { get; }

        public ICategoryDao Categories { get; }

        public IProductDao Products { get; }

        public IRoleDao Roles { get; }

        public IUserDao Users { get; }
    }
}
