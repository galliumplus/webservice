﻿using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Core.Sales
{
    public class PaymentMethodFactory
    {
        private IUserDao userDao;

        public PaymentMethodFactory(IUserDao userDao)
        {
            this.userDao = userDao;
        }

        public PaymentMethod Create(string method, string? customer)
        {
            switch (method)
            {
                case "CREDIT_CARD":
                    return new PaymentByCreditCard();

                case "DEPOSIT":
                    if (customer == null || customer == "@anonymousmember") {
                        throw new InvalidItemException(
                            "L'identifiant de l'adhérent est obligatoire pour les paiments en acompte."
                        );
                    }

                    return new PaymentByDeposit(userDao, customer);

                case "PAYPAL":
                    return new PaymentByPaypal();

                case "CASH":
                    return new PaymentInCash();

                default:
                    throw new InvalidItemException($"Type de paiement « {method} » invalide");
            }
        }
    }
}