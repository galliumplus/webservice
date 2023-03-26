namespace GalliumPlus.WebApi.Core.Data
{
    public interface IMasterDao
    {
        public IRoleDao Roles { get; }

        public IUserDao Users { get; }
    }
}
