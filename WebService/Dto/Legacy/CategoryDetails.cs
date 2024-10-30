using System.ComponentModel.DataAnnotations;
using GalliumPlus.Core.Stocks;

namespace GalliumPlus.WebService.Dto.Legacy
{
    public class CategoryDetails
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }

        public CategoryDetails()
        {
            this.Id = -1;
            this.Name = String.Empty;
        }

        public class Mapper : Mapper<Category, CategoryDetails>
        {
            public override CategoryDetails FromModel(Category model)
            {
                return new CategoryDetails { Id = model.Id, Name = model.Name };
            }

            public override Category ToModel(CategoryDetails dto)
            {
                return new Category(dto.Id, dto.Name);
            }
        }
    }
}
