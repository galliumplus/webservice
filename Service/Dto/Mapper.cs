namespace GalliumPlus.WebApi.Dto
{
    public abstract class Mapper<TModel, TDto>
    {
        public abstract TModel ToModel(TDto dto);

        public abstract TDto FromModel(TModel model);

        public IEnumerable<TDto> FromModel(IEnumerable<TModel> models)
        {
            foreach (TModel model in models) yield return this.FromModel(model);
        }
    }
}
