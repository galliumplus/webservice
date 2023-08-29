using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace GalliumPlus.WebApi.Core.Orders
{
    public class PaymentByDeposit : PaymentMethod
    {
        private static readonly Lazy<IFormatProvider> currencyFormat
            = new Lazy<IFormatProvider>(() => CultureInfo.GetCultureInfo("fr-FR").NumberFormat);

        private IUserDao userDao;
        private string depositId;

        /// <summary>
        /// L'identifiant de l'utilisateur qui effectue le paiement.
        /// </summary>
        public string UserId => this.depositId;

        /// <summary>
        /// Crée un mode de paiement par acompte.
        /// </summary>
        /// <param name="userDao">Un DAO des utilisateurs.</param>
        /// <param name="userId">L'acompte avec lequel payer.</param>
        public PaymentByDeposit(IUserDao userDao, string depositId)
        {
            this.userDao = userDao;
            this.depositId = depositId;
        }

        protected override string ProcessPayment(double amount)
        {
            double currentDeposit;
            try
            {
                currentDeposit = this.userDao.ReadDeposit(this.depositId);
            }
            catch (ItemNotFoundException)
            {
                throw new CantSellException("Cet utilisateur n'existe pas.");
            }

            double newDeposit = currentDeposit - amount;
            if (newDeposit < 0)
            {
                throw new CantSellException(
                    string.Format(
                        currencyFormat.Value,
                        "Il n'y a pas assez d'argent sur l'acompte ({0:C} au lieu de {1:C}).",
                        currentDeposit, amount
                    )
                );
            }

            this.userDao.UpdateDeposit(this.depositId, newDeposit);

            return string.Format(
                currencyFormat.Value,
                "Le paiement par acompte a bien été effectué (il reste {0:C}).",
                newDeposit
            );
        }
    }
}
