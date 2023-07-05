using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Sales;
using GalliumPlus.WebApi.Core.Users;
using System.ComponentModel.DataAnnotations;

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

        public class Mapper : Mapper<Sale, SaleSummary, (IProductDao, IUserDao)>
        {
            private SaleItemSummary.Mapper saleItemMapper = new();

            public override SaleSummary FromModel(Sale model)
            {
                throw new NotImplementedException();
            }

            public override Sale ToModel(SaleSummary dto, (IProductDao, IUserDao) daos)
            {
                (IProductDao productDao, IUserDao userDao) = daos;
                PaymentMethodFactory factory = new(daos.Item2);

                User? customer;
                if (dto.Customer == null)
                {
                    customer = null;
                }
                else if (dto.Customer == Sale.ANONYMOUS_MEMBER_ID)
                {
                    customer = BuildAnonymousMember();
                }
                else
                {
                    customer = userDao.Read(dto.Customer);
                }

                return new Sale(
                    factory.Create(dto.PaymentMethod, dto.Customer),
                    dto.Items!.Select(saleItemDto => saleItemMapper.ToModel(saleItemDto, daos.Item1)),
                    customer
                );
            }

            private static User BuildAnonymousMember()
            {
                return new User(
                    Sale.ANONYMOUS_MEMBER_ID,
                    "Anonyme",
                    new Role(-1, "Membre anonyme", Permissions.NONE),
                    "Anonyme",
                    0.00,
                    false
                );
            }
        }
    }
}
