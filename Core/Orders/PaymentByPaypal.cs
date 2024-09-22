namespace GalliumPlus.WebApi.Core.Orders;

public class PaymentByPaypal : PaymentMethod
{
    public override string Description => "par PayPal";

    protected override string ProcessPayment(decimal _) => "OK";
}