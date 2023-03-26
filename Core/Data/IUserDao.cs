namespace GalliumPlus.WebApi.Core.Data
{
    public interface IUserDao : IBasicDao<string, User>
    {
        public void UpdateDeposit(string id, double deposit);
    }
}
