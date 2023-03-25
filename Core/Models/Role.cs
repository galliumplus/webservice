using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Models
{
    /// <summary>
    /// Le rôle d'un utilisateur. 
    /// </summary>
    public class Role
    {
        /// <summary>
        /// L'identifiant du rôle.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Le nom affiché du rôle.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// La somme des permissions attribuées au rôle.
        /// </summary>
        private int Permissions { get; set; }

        /// <summary>
        /// Vérifie que le rôle a une certaine permission.
        /// </summary>
        /// <param name="perm">La permission à vérifier</param>
        /// <returns><see langword="true"/> si le rôle a la permission, sinon <see langword="false"/>.</returns>
        public bool HasPermission(Permission perm)
        {
            return (this.Permissions & (int)perm) != 0;
        }

        /// <summary>
        /// Crée un rôle.
        /// </summary>
        /// <param name="id">L'identifiant du rôle.</param>
        /// <param name="name">Le nom affiché du rôle.</param>
        /// <param name="permissions">La somme des permissions attribuées au rôle.</param>
        public Role(int id, string name, int permissions)
        {
            this.Id = id;
            this.Name = name;
            this.Permissions = permissions;
        }
    }
}