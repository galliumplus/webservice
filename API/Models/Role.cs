using System.Text.Json.Serialization;

namespace GalliumPlusAPI.Models
{
	/// <summary>
	/// Le rôle d'un utilisateur. 
	/// </summary>
	public class Role : IModel<int>
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
		public int Permissions { get; set; }

        /// <summary>
        /// Vérifie qu'un rôle a une certaine permission.
        /// </summary>
        /// <param name="perm">La permission à vérifier</param>
        /// <returns><see langword="true"/> si le rôle a la permission, sinon <see langword="false"/>.</returns>
        public bool HasPermission(Permission perm)
		{
			return (this.Permissions & (int) perm) != 0;
		}
	}
}