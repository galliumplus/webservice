using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Users
{
    /// <summary>
    /// Les informations personelles d'un utilisateur.
    /// </summary>
    public class UserIdentity
    {
        private string firstName;
        private string lastName;
        private string email;
        private string year;

        /// <summary>
        /// Le prénom de l'utilisateur.
        /// </summary>
        public string FirstName { get => this.firstName; set => this.firstName = value; }

        /// <summary>
        /// Le nom de l'utilisateur.
        /// </summary>
        public string LastName { get => this.lastName; set => this.lastName = value; }

        /// <summary>
        /// L'addresse mail de l'utilisateur.
        /// </summary>
        public string Email => this.email;

        /// <summary>
        /// La promotion de l'utilisateur.
        /// </summary>
        public string Year { get => this.year; set => this.year = value; }

        public UserIdentity(string firstName, string lastName, string email, string year)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.year = year;
        }
    }
}
