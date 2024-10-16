using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Orders;
using GalliumPlus.Core.Stocks;
using GalliumPlus.Core.Users;
using GalliumPlus.Data.Fake;

namespace GalliumPlus.WebApi.Core.Orders;

public class OrderTest
{
    [Fact]
    public void Empty()
    {
            var products = new ProductDao(new CategoryDao());

            Order order = new(
                new PaymentByPaypal(),
                new List<OrderItem> { }
            );

            Assert.Throws<CantSellException>(() => order.ProcessPaymentAndUpdateStock(products));
        }

    [Fact]
    public void NotEnoughInStock()
    {
            var products = new ProductDao(new CategoryDao());

            Product product1 = products.Read(1);
            Product product2 = products.Read(2);

            Order order = new(
                new PaymentByPaypal(),
                new List<OrderItem> {
                    new OrderItem(product1, product1.Stock / 2),
                    new OrderItem(product2, product2.Stock * 2),
                }
            );

            Assert.Throws<CantSellException>(() => order.ProcessPaymentAndUpdateStock(products));
        }

    [Fact]
    public void PaymentByDeposit()
    {
            var products = new ProductDao(new CategoryDao());
            var users = new UserDao(new RoleDao());

            Product product = products.Read(1);

            User customer = users.Read("lomens");
            customer.IsMember = false;
            customer.Deposit = product.MemberPrice;
            users.Update("lomens", customer);

            Order order = new(
                new PaymentByDeposit(users, "lomens"),
                new List<OrderItem> {
                    new OrderItem(product, 1),
                },
                customer
            );

            Assert.Throws<CantSellException>(() => order.ProcessPaymentAndUpdateStock(products));

            customer.IsMember = true;
            users.Update("lomens", customer);

            order.ProcessPaymentAndUpdateStock(products);
        }

    [Fact]
    public void OrderProcessing()
    {
            var products = new ProductDao(new CategoryDao());
            var users = new UserDao(new RoleDao());

            Product product1 = products.Read(1);
            Product product2 = products.Read(2);

            User customer = users.Read("lomens");
            customer.IsMember = true;
            customer.Deposit = 6000;
            users.Update("lomens", customer);

            Order order = new(
                new PaymentByDeposit(users, "lomens"),
                new List<OrderItem> {
                    new OrderItem(product1, 1),
                    new OrderItem(product2, 2),
                },
                customer
            );

            decimal? depositBefore = users.ReadDeposit("lomens");
            int stock1Before = products.Read(1).Stock;
            int stock2Before = products.Read(2).Stock;

            order.ProcessPaymentAndUpdateStock(products);

            decimal? depositAfter = users.ReadDeposit("lomens");
            int stock1After = products.Read(1).Stock;
            int stock2After = products.Read(2).Stock;

            decimal? expectedPrice = product1.MemberPrice + product2.MemberPrice * 2;
            Assert.Equal(depositAfter, depositBefore - expectedPrice);
            Assert.Equal(stock1After, stock1Before - 1);
            Assert.Equal(stock2After, stock2Before - 2);
        }
}