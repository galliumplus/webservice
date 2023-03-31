using GalliumPlus.WebApi.Core.Serialization;
using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Core
{
    /// <summary>
    /// Le rôle d'un utilisateur. 
    /// </summary>
    public class Role
    {
        private int id;
        private string name;
        private Permission permissions;

        /// <summary>
        /// L'identifiant du rôle.
        /// </summary>
        [JsonReferenceKey]
        public int Id { get => this.id; set => this.id = value; }

        /// <summary>
        /// Le nom affiché du rôle.
        /// </summary>
        public string Name { get => this.name; set => this.name = value; }

        /// <summary>
        /// La somme des permissions attribuées au rôle.
        /// </summary>
        private Permission Permissions { get => this.permissions; set => this.permissions = value; }

        /// <summary>
        /// Vérifie que le rôle a une certaine permission.
        /// </summary>
        /// <param name="perm">La permission à vérifier</param>
        /// <returns><see langword="true"/> si le rôle a la permission, sinon <see langword="false"/>.</returns>
        public bool HasPermission(Permission perm)
        {
            return (Permissions & perm) != 0;
        }

        /// <summary>
        /// Crée un rôle.
        /// </summary>
        /// <param name="id">L'identifiant du rôle.</param>
        /// <param name="name">Le nom affiché du rôle.</param>
        /// <param name="permissions">La somme des permissions attribuées au rôle.</param>
        public Role(int id, string name, Permission permissions)
        {
            this.id = id;
            this.name = name;
            this.permissions = permissions;
        }
    }
}