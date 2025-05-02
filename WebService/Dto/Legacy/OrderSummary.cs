using System.ComponentModel.DataAnnotations;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Orders;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Users;

namespace GalliumPlus.WebService.Dto.Legacy
{
    public class OrderSummary
    {
        [Required] public string PaymentMethod { get; set; }
        public string? Customer { get; set; }
        [Required] public List<OrderItemSummary>? Items { get; set; }

        public OrderSummary()
        {
            this.PaymentMethod = String.Empty;
            this.Items = null;
        }

        public class Mapper : Mapper<Order, OrderSummary>
        {
            private OrderItemSummary.Mapper orderItemMapper;
            private IUserDao userDao;

            public Mapper(IUserDao userDao, IProductDao productDao)
            {
                this.userDao = userDao;
                this.orderItemMapper = new(productDao);
            }

            public override OrderSummary FromModel(Order model)
            {
                // ne sort jamais du serveur !
                throw new NotImplementedException();
            }

            public override Order ToModel(OrderSummary dto)
            {
                PaymentMethodFactory factory = new(this.userDao);

                User? customer;
                if (dto.Customer == null)
                {
                    customer = null;
                }
                else if (dto.Customer == Order.ANONYMOUS_MEMBER_ID)
                {
                    customer = BuildAnonymousMember();
                }
                else
                {
                    try
                    {
                        customer = this.userDao.Read(dto.Customer);
                    }
                    catch (ItemNotFoundException)
                    {
                        throw new InvalidResourceException($"L'utilisateur « {dto.Customer} » n'existe pas");
                    }
                }

                return new Order(
                    factory.Create(dto.PaymentMethod, dto.Customer),
                    dto.Items!.Select(saleItemDto => this.orderItemMapper.ToModel(saleItemDto)),
                    customer
                );
            }

            private static User BuildAnonymousMember()
            {
                return new User(
                    999999999,
                    "anonymousmember00000000000", // pas possible d'être rentré en BDD
                    new UserIdentity("Anonyme", "", "", ""),
                    new Role(-1, "Membre anonyme", Permission.None),
                    0.00m,
                    true
                );
            }
        }
    }
}
