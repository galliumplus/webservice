namespace GalliumPlus.WebApi.Core
{
    /// <summary>
    /// Un utilisateur de l'application.
    /// </summary>
    public class User
    {
        /// <summary>
        /// L'identifiant IUT (ou identifiant spécial) de l'utilisateur.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Le prénom et nom de l'utilisateur.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// L'identifiant du rôle de l'utilisateur.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// La promotion de l'utilisateur.
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// L'acompte de l'utilisateur.
        /// </summary>
        public double Deposit { get; set; }

        /// <summary>
        /// Crée un utilisateur.
        /// </summary>
        /// <param name="id">L'identifiant IUT (ou identifiant spécial) de l'utilisateur.</param>
        /// <param name="name">Le prénom et nom de l'utilisateur.</param>
        /// <param name="roleId">L'identifiant du rôle de l'utilisateur.</param>
        /// <param name="year">La promotion de l'utilisateur.</param>
        /// <param name="deposit">L'acompte de l'utilisateur.</param>
        public User(string id, string name, int roleId, string year, double deposit)
        {
            Id = id;
            Name = name;
            RoleId = roleId;
            Year = year;
            Deposit = deposit;
        }
    }
}