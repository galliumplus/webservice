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
        private bool formerMember;
        private PasswordInformation? password;

        /// <summary>
        /// L'identifiant IUT (ou identifiant spécial) de l'utilisateur.
        /// </summary>
        public string Id { get => this.id; set => this.id = value; }

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
        /// L'acompte de l'utilisateur.
        /// </summary>
        public double Deposit { get => this.deposit; set => this.deposit = value; }

        /// <summary>
        /// <see langword="true"/> si l'utilisateur n'est plus adhérent.
        /// </summary>
        public bool FormerMember { get => this.formerMember; set => this.formerMember = value; }

        /// <summary>
        /// Crée un utilisateur sans informations sur son mot de passe.
        /// </summary>
        /// <param name="id">L'identifiant IUT (ou identifiant spécial) de l'utilisateur.</param>
        /// <param name="name">Le prénom et nom de l'utilisateur.</param>
        /// <param name="roleId">L'identifiant du rôle de l'utilisateur.</param>
        /// <param name="year">La promotion de l'utilisateur.</param>
        /// <param name="deposit">L'acompte de l'utilisateur.</param>
        /// <param name="formerMember"> <see langword="true"/> si l'utilisateur n'est plus adhérent.</param>
        public User(
            string id,
            string name,
            Role role,
            string year,
            double deposit,
            bool formerMember)
        {
            this.id = id;
            this.name = name;
            this.role = role;
            this.year = year;
            this.deposit = deposit;
            this.formerMember = formerMember;
            this.password = null;
        }

        /// <summary>
        /// Crée un utilisateur avec les informations de mot de passe.
        /// </summary>
        /// <param name="id">L'identifiant IUT (ou identifiant spécial) de l'utilisateur.</param>
        /// <param name="name">Le prénom et nom de l'utilisateur.</param>
        /// <param name="roleId">L'identifiant du rôle de l'utilisateur.</param>
        /// <param name="year">La promotion de l'utilisateur.</param>
        /// <param name="deposit">L'acompte de l'utilisateur.</param>
        /// <param name="formerMember"> <see langword="true"/> si l'utilisateur n'est plus adhérent.</param>
        /// <param name="password">Le mot de passe de l'utilisateur.</param>
        public User(
            string id,
            string name,
            Role role,
            string year,
            double deposit,
            bool formerMember,
            PasswordInformation password)
        : this(id, name, role, year, deposit, formerMember)
        {
            this.password = password;
        }
    }
}