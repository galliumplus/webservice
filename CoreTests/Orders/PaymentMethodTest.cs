using GalliumPlus.WebApi.Core.Exceptions;

namespace GalliumPlus.WebApi.Core.Orders;

public class PaymentMethodTest
{
    private void ExternalPayment(PaymentMethod method)
    {
            method.Pay(0);
            method.Pay(10);
            method.Pay(1_000_000_000);
            Assert.Throws<ArgumentOutOfRangeException>(() => method.Pay(-10));
        }

    [Fact]
    public void ByCreditCard()
    {
            this.ExternalPayment(new PaymentByCreditCard());
        }

    [Fact]
    public void ByPaypal()
    {
            this.ExternalPayment(new PaymentByPaypal());
        }

    [Fact]
    public void InCash()
    {
            this.ExternalPayment(new PaymentInCash());
        }

    [Fact]
    public void ByDeposit()
    {
            var users = new UserDao(new RoleDao());

            PaymentMethod withRealCustomer = new PaymentByDeposit(users, "lomens");

            User customer = users.Read("lomens");

            // Free

            decimal? before = users.ReadDeposit("lomens");
            withRealCustomer.Pay(0);
            decimal? after = users.ReadDeposit("lomens");
            Assert.Equal(before, after);

            // Enough deposit

            customer.Deposit = 10;
            users.Update("lomens", customer);
            withRealCustomer.Pay(6.80m);
            after = users.ReadDeposit("lomens");
            Assert.Equal(3.20m, after);

            // Not enough deposit

            customer.Deposit = 10;
            users.Update("lomens", customer);
            Assert.Throws<CantSellException>(() => withRealCustomer.Pay(24.20m));
            after = users.ReadDeposit("lomens");
            Assert.Equal(10, after);

            // Negative payment

            customer.Deposit = 10;
            users.Update("lomens", customer);
            Assert.Throws<ArgumentOutOfRangeException>(() => withRealCustomer.Pay(-6.80m));
            after = users.ReadDeposit("lomens");
            Assert.Equal(10, after);

            // Invalid ID

            PaymentMethod withFakeCustomer = new PaymentByDeposit(users, "jj000000");

            Assert.Throws<CantSellException>(() => withFakeCustomer.Pay(0));
        }
}