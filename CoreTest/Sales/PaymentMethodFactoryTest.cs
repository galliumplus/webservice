namespace CoreTest.Sales
{
    public class PaymentMethodFactoryTest
    {
        private void CreateExternal<ExactType>(string method)
        {
            var factory = new PaymentMethodFactory(new UserDao(new RoleDao()));

            var paymentByCreditCard = factory.Create(method, null);
            Assert.IsType<ExactType>(paymentByCreditCard);

            paymentByCreditCard = factory.Create(method, "@anonymousmember");
            Assert.IsType<ExactType>(paymentByCreditCard);

            paymentByCreditCard = factory.Create(method, "lomens");
            Assert.IsType<ExactType>(paymentByCreditCard);
        }

        [Fact]
        public void CreateCreditCard()
        {
            CreateExternal<PaymentByCreditCard>("CREDIT_CARD");
        }

        [Fact]
        public void CreatePaypal()
        {
            CreateExternal<PaymentByPaypal>("PAYPAL");
        }

        [Fact]
        public void CreateCash()
        {
            CreateExternal<PaymentInCash>("CASH");
        }

        [Fact]
        public void CreateDeposit()
        {
            var factory = new PaymentMethodFactory(new UserDao(new RoleDao()));

            Assert.Throws<InvalidItemException>(() => factory.Create("DEPOSIT", null));

            Assert.Throws<InvalidItemException>(() => factory.Create("DEPOSIT", "@anonymousmember"));

            var withRealCustomer = factory.Create("DEPOSIT", "lomens");
            var withFakeCustomer = factory.Create("DEPOSIT", "jj000000");

            Assert.IsType<PaymentByDeposit>(withRealCustomer);
            Assert.IsType<PaymentByDeposit>(withFakeCustomer);
        }
    }
}
