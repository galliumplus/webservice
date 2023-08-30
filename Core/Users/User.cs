using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Core.Users
{
    /// <summary>
    /// Un utilisateur de l'application.
    /// </summary>
    public class User
    {
        private string id;
        private string name;
        private Role role;
        private string year;
        private double deposit;
        private bool isMember;
        private PasswordInformation? password;

        /// <summary>
        /// L'identifiant IUT (ou identifiant spécial) de l'utilisateur.
        /// </summary>
        /// <exception cref="InvalidItemException">Le setter renvoie une erreur si la valeur contient des caractères speciaux</exception>
        public string Id 
        { 
            get => this.id; 
            set 
            {
                if (value.Any(c => !char.IsLetterOrDigit(c)))
                    throw new InvalidItemException("Un identifiant ne peut pas contenir de caractères speciaux");
                else
                    this.id = value;
            } 
        }

        /// <summary>
        /// Le prénom et nom de l'utilisateur.
        /// </summary>
        public string Name { get => this.name; set => this.name = value; }

        /// <summary>
        /// L'identifiant du rôle de l'utilisateur.
        /// </summary>
        public Role Role { get => this.role; set => this.role = value; }

        /// <summary>
        /// Le mot de passe de l'ulitlisateur.
        /// </summary>
        /// <remarks>
        /// Cette propriété ne doit pas être exposée par l'API.
        /// </remarks>
        public PasswordInformation? Password { get => this.password; set => this.password = value; }

        /// <summary>
        /// La promotion de l'utilisateur.
        /// </summary>
        public string Year { get => this.year; set => this.year = value; }

        /// <summary>
        /// L'acompte de l'utilisateur en euros.
        /// </summary>
        public double Deposit { get => this.deposit; set => this.deposit = value; }

        /// <summary>
        /// <see langword="true"/> si l'utilisateur n'est plus adhérent.
        /// </summary>
        public bool IsMember { get => this.isMember; set => this.isMember = value; }

        /// <summary>
        /// Crée un utilisateur sans informations sur son mot de passe.
        /// </summary>
        /// <param name="id">L'identifiant IUT (ou identifiant spécial) de l'utilisateur.</param>
        /// <param name="name">Le prénom et nom de l'utilisateur.</param>
        /// <param name="role">Le rôle de l'utilisateur.</param>
        /// <param name="year">La promotion de l'utilisateur.</param>
        /// <param name="deposit">L'acompte de l'utilisateur en euros.</param>
        /// <param name="isMember"> <see langword="true"/> si l'utilisateur est adhérent.</param>
        /// <exception cref="InvalidItemException">Renvoie une erreur si User.Id contient des caractères speciaux</exception>
        public User(
            string id,
            string name,
            Role role,
            string year,
            double deposit,
            bool isMember)
        {
            this.Id = id;
            this.name = name;
            this.role = role;
            this.year = year;
            this.deposit = deposit;
            this.isMember = isMember;
            this.password = null;
        }

        /// <summary>
        /// Crée un utilisateur avec les informations de mot de passe.
        /// </summary>
        /// <param name="id">L'identifiant IUT (ou identifiant spécial) de l'utilisateur.</param>
        /// <param name="name">Le prénom et nom de l'utilisateur.</param>
        /// <param name="roleId">L'identifiant du rôle de l'utilisateur.</param>
        /// <param name="year">La promotion de l'utilisateur.</param>
        /// <param name="deposit">L'acompte de l'utilisateur en euros.</param>
        /// <param name="isMember"> <see langword="true"/> si l'utilisateur est adhérent.</param>
        /// <param name="password">Le mot de passe de l'utilisateur.</param>
        /// <exception cref="InvalidItemException">Renvoie une erreur si User.Id contient des caractères speciaux</exception>
        public User(
            string id,
            string name,
            Role role,
            string year,
            double deposit,
            bool isMember,
            PasswordInformation password)
        : this(id, name, role, year, deposit, isMember)
        {
            this.password = password;
        }
    }
}