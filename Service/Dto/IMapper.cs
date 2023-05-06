using GalliumPlus.WebApi.Core.Data;

namespace GalliumPlus.WebApi.Dto
{
    public abstract class IMapper<TModel, TDto>
    {
        public abstract TModel ToModel(TDto dto, IMasterDao dao);

        public abstract TDto FromModel(TModel model);

        public IEnumerable<TDto> FromModel(IEnumerable<TModel> models)
        {
            foreach (TModel model in models) yield return this.FromModel(model);
        }
    }
}
