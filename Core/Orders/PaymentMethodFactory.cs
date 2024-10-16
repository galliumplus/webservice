using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;

namespace GalliumPlus.Core.Orders;

public class PaymentMethodFactory
{
    private IUserDao userDao;

    public PaymentMethodFactory(IUserDao userDao)
    {
        this.userDao = userDao;
    }

    public PaymentMethod Create(string method, string? customer)
    {
        switch (method.ToLower())
        {
        case "creditcard":
        case "credit_card":
            return new PaymentByCreditCard();

        case "deposit":
            if (customer == null || customer == "@anonymousmember")
            {
                throw new InvalidItemException(
                    "L'identifiant de l'adhérent est obligatoire pour les paiments par acompte."
                );
            }

            return new PaymentByDeposit(this.userDao, customer);

        case "paypal":
            return new PaymentByPaypal();

        case "cash":
            return new PaymentInCash();

        default:
            throw new InvalidItemException($"Type de paiement « {method} » invalide");
        }
    }
}