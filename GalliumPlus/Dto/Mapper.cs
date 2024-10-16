namespace GalliumPlus.WebApi.Dto
{
    public abstract class Mapper<TModel, TDto>
    {
        public virtual TModel ToModel(TDto dto) 
        {
            throw new InvalidOperationException($"Les DTOs de type {typeof(TDto).Name} peuvent être utilisés seulement en envoi.");
        }

        public virtual TDto FromModel(TModel model) 
        {
            throw new InvalidOperationException($"Les DTOs de type {typeof(TDto).Name} peuvent être utilisés seulement en réception.");
        }

        public IEnumerable<TDto> FromModel(IEnumerable<TModel> models)
        {
            foreach (TModel model in models) yield return this.FromModel(model);
        }
    }
}
