namespace GalliumPlus.WebApi.Core.Orders;

public class PaymentInCash : PaymentMethod
{
    public override string Description => "en liquide";

    protected override string ProcessPayment(decimal _) => "OK";
}