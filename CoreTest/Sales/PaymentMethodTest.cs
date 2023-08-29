using GalliumPlus.WebApi.Core.Users;

namespace CoreTest.Sales
{
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
            ExternalPayment(new PaymentByCreditCard());
        }

        [Fact]
        public void ByPaypal()
        {
            ExternalPayment(new PaymentByPaypal());
        }

        [Fact]
        public void InCash()
        {
            ExternalPayment(new PaymentInCash());
        }

        [Fact]
        public void ByDeposit()
        {
            var users = new UserDao(new RoleDao());

            PaymentMethod withRealCustomer = new PaymentByDeposit(users, "lomens");

            // Free

            double before = users.ReadDeposit("lomens");
            withRealCustomer.Pay(0);
            double after = users.ReadDeposit("lomens");
            Assert.Equal(before, after);

            // Enough deposit

            users.UpdateDeposit("lomens", 10);
            withRealCustomer.Pay(6.80);
            after = users.ReadDeposit("lomens");
            Assert.Equal(3.20, after);

            // Not enough deposit

            users.UpdateDeposit("lomens", 10);
            Assert.Throws<CantSellException>(() => withRealCustomer.Pay(24.20));
            after = users.ReadDeposit("lomens");
            Assert.Equal(10, after);

            // Negative payment

            users.UpdateDeposit("lomens", 10);
            Assert.Throws<ArgumentOutOfRangeException>(() => withRealCustomer.Pay(-6.80));
            after = users.ReadDeposit("lomens");
            Assert.Equal(10, after);

            // Invalid ID

            PaymentMethod withFakeCustomer = new PaymentByDeposit(users, "jj000000");

            Assert.Throws<CantSellException>(() => withFakeCustomer.Pay(0));
        }
    }
}
