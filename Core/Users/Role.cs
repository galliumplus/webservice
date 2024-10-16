namespace GalliumPlus.Core.Users;

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
    public int Id { get => this.id; set => this.id = value; }

    /// <summary>
    /// Le nom affiché du rôle.
    /// </summary>
    public string Name { get => this.name; set => this.name = value; }

    /// <summary>
    /// La somme des permissions attribuées au rôle.
    /// </summary>
    public Permissions Permissions { get => this.permissions; set => this.permissions = value; }

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