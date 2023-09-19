namespace GalliumPlus.WebApi.Core.Orders
{
    public class PaymentByCreditCard : PaymentMethod
    {
        public override string Description => "carte bancaire";

        protected override string ProcessPayment(decimal _) => "OK";
    }
}
