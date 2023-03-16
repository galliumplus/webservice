namespace GalliumPlusAPI.Database
{
    public interface IDao
    {
        IProductDao Products { get; }

        ICategoryDao Categories { get; }
    }
}
