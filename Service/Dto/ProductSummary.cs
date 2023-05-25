using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Stocks;
using GalliumPlus.WebApi.Core.Users;
using System.ComponentModel.DataAnnotations;

namespace GalliumPlus.WebApi.Dto
{
    public class ProductSummary
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public int? Stock { get; set; }
        [Required] public double? NonMemberPrice { get; set; }
        [Required] public double? MemberPrice { get; set; }
        [Required] public Availability? Availability { get; set; }
        [Required] public int? Category { get; set; }

        public ProductSummary()
        {
            this.Id = -1;
            this.Name = string.Empty;
        }

        public class Mapper : Mapper<Product, ProductSummary, IProductDao>
        {
            public override ProductSummary FromModel(Product model)
            {
                return new ProductSummary
                {
                    Id = model.Id,
                    Name = model.Name,
                    Stock = model.Stock,
                    NonMemberPrice = model.NonMemberPrice,
                    MemberPrice = model.MemberPrice,
                    Availability = model.Availability,
                    Category = model.Category.Id
                };
            }

            public override Product ToModel(ProductSummary dto, IProductDao dao)
            {
                Category category;
                try
                {
                    category = dao.Categories.Read(dto.Category!.Value);
                }
                catch (ItemNotFoundException)
                {
                    throw new InvalidItemException("La catégorie associée n'existe pas");
                }

                return new Product(
                    dto.Id, dto.Name,
                    dto.Stock!.Value,
                    dto.NonMemberPrice!.Value,
                    dto.MemberPrice!.Value,
                    dto.Availability!.Value,
                    category
                );
            }
        }
    }
}
