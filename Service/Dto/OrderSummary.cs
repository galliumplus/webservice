using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Orders;
using GalliumPlus.WebApi.Core.Users;
using System.ComponentModel.DataAnnotations;

namespace GalliumPlus.WebApi.Dto
{
    public class OrderSummary
    {
        [Required] public string PaymentMethod { get; set; }
        public string? Customer { get; set; }
        [Required] public List<OrderItemSummary>? Items { get; set; }

        public OrderSummary()
        {
            PaymentMethod = String.Empty;
            Items = null;
        }

        public class Mapper : Mapper<Order, OrderSummary, (IProductDao, IUserDao)>
        {
            private OrderItemSummary.Mapper saleItemMapper = new();

            public override OrderSummary FromModel(Order model)
            {
                // ne sort jamais du serveur !
                throw new NotImplementedException();
            }

            public override Order ToModel(OrderSummary dto, (IProductDao, IUserDao) daos)
            {
                (IProductDao productDao, IUserDao userDao) = daos;
                PaymentMethodFactory factory = new(daos.Item2);

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
                        customer = userDao.Read(dto.Customer);
                    }
                    catch (ItemNotFoundException)
                    {
                        throw new InvalidItemException($"L'utilisateur « {dto.Customer} » n'existe pas");
                    }
                }

                return new Order(
                    factory.Create(dto.PaymentMethod, dto.Customer),
                    dto.Items!.Select(saleItemDto => saleItemMapper.ToModel(saleItemDto, daos.Item1)),
                    customer
                );
            }

            private static User BuildAnonymousMember()
            {
                return new User(
                    "anonymous",
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
