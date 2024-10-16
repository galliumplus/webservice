using System.ComponentModel.DataAnnotations;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Orders;

namespace GalliumPlus.WebService.Dto
{
    public class OrderItemSummary
    {
        [Required] public int? Product { get; set; }
        [Required] public int? Quantity { get; set; }

        public class Mapper : Mapper<OrderItem, OrderItemSummary>
        {
            private IProductDao productDao;

            public Mapper(IProductDao productDao)
            {
                this.productDao = productDao;
            }

            public override OrderItemSummary FromModel(OrderItem model)
            {
                // ne sort jamais du serveur !
                throw new NotImplementedException();
            }

            public override OrderItem ToModel(OrderItemSummary dto)
            {
                return new OrderItem(this.productDao.Read(dto.Product!.Value), dto.Quantity!.Value);
            }
        }
    }
}
