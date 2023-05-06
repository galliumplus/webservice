namespace GalliumPlus.WebApi.Core.Users
{
    /// <summary>
    /// Le rôle d'un utilisateur. 
    /// </summary>
    public class Role
    {
        private int id;
        private string name;
        private Permissions permissions;

        /// <summary>
        /// L'identifiant du rôle.
        /// </summary>
        public int Id { get => this.id; set => id = value; }

        /// <summary>
        /// Le nom affiché du rôle.
        /// </summary>
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// La somme des permissions attribuées au rôle.
        /// </summary>
        private Permissions Permissions { get => permissions; set => permissions = value; }

        /// <summary>
        /// Vérifie que le rôle a une certaine permission.
        /// </summary>
        /// <param name="perm">La permission à vérifier</param>
        /// <returns><see langword="true"/> si le rôle a la permission, sinon <see langword="false"/>.</returns>
        public bool HasPermission(Permissions perm)
        {
            return (Permissions & perm) != 0;
        }

        /// <summary>
        /// Crée un rôle.
        /// </summary>
        /// <param name="id">L'identifiant du rôle.</param>
        /// <param name="name">Le nom affiché du rôle.</param>
        /// <param name="permissions">La somme des permissions attribuées au rôle.</param>
        public Role(int id, string name, Permissions permissions)
        {
            this.id = id;
            this.name = name;
            this.permissions = permissions;
        }
    }
}