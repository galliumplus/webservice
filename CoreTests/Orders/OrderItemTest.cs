using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Orders;
using GalliumPlus.Core.Stocks;
using GalliumPlus.Data.Fake;

namespace GalliumPlus.WebApi.Core.Orders;

public class OrderItemTest
{
    [Fact]
    public void Tests()
    {
            var products = new ProductDao(new CategoryDao());
            Product product = products.Read(1);

            Assert.Throws<InvalidItemException>(() => new OrderItem(product, 0));

            Assert.Throws<InvalidItemException>(() => new OrderItem(product, -3));

            OrderItem item = new OrderItem(product, 3);

            Assert.Equal(product, item.Product);
            Assert.Equal(3, item.Quantity);
            Assert.Equal(product.NonMemberPrice, item.NonMemberUnitPrice);
            Assert.Equal(product.MemberPrice, item.MemberUnitPrice);
            Assert.Equal(product.NonMemberPrice * 3, item.NonMemberTotalPrice);
            Assert.Equal(product.MemberPrice * 3, item.MemberTotalPrice);
        }
}