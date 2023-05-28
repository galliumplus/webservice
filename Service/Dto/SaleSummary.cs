using GalliumPlus.WebApi.Core.Data;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace GalliumPlus.WebApi.Dto
{
    public class SaleSummary
    {
        [Required] public string PaymentMethod { get; set; }
        public string? Customer { get; set; }
        [Required] public List<SaleItemSummary>? Items { get; set; }

        public SaleSummary()
        {
            PaymentMethod = String.Empty;
            Items = null;
        }

        public class Mapper : Mapper<int, SaleSummary, (IProductDao, IUserDao)>
        {
            public override SaleSummary FromModel(int model)
            {
                throw new NotImplementedException();
            }

            public override int ToModel(SaleSummary dto, (IProductDao, IUserDao) daos)
            {
                foreach (SaleItemSummary item in dto.Items)
                {
                    Console.Write(item);
                    Console.Write(", ");
                }
                Console.WriteLine();

                return 4;
            }
        }
    }
}
