namespace GalliumPlus.WebService.Dto.Legacy
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
            return models.Select(this.FromModel);
        }
    }
}
