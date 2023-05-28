using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Sales;
using System.ComponentModel.DataAnnotations;

namespace GalliumPlus.WebApi.Dto
{
    public class SaleItemSummary
    {
        [Required] public int? Product { get; set; }
        [Required] public int? Quantity { get; set; }

        public class Mapper : Mapper<SaleItem, SaleItemSummary, IProductDao>
        {
            public override SaleItemSummary FromModel(SaleItem model)
            {
                // ne sort jamais du serveur !
                throw new NotImplementedException();
            }

            public override SaleItem ToModel(SaleItemSummary dto, IProductDao dao)
            {
                return new SaleItem(dao.Read(dto.Product!.Value), dto.Quantity!.Value);
            }
        }
    }
}
