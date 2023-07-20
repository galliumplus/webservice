namespace GalliumPlus.WebApi.Dto
{
    public abstract class Mapper<TModel, TDto, TDao>
    {
        public abstract TModel ToModel(TDto dto, TDao dao);

        public abstract TDto FromModel(TModel model);

        public IEnumerable<TDto> FromModel(IEnumerable<TModel> models)
        {
            foreach (TModel model in models) yield return this.FromModel(model);
        }
    }

    public static class MapperExtensions
    {
        public static IServiceCollection AddMapper<TMapper>(this IServiceCollection @this)
        where TMapper : class
        {
            return @this.AddTransient<TMapper>();
        }
    }
}
