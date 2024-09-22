namespace GalliumPlus.WebApi.Core.Data.HistorySearch;

/// <summary>
/// Décris la position d'une vue dans une liste.
/// </summary>
public readonly struct Pagination
{
    private readonly int pageIndex;
    private readonly int pageSize;

    /// <summary>
    /// L'index de la page. (commence à zéro)
    /// </summary>
    public int PageIndex => this.pageIndex;

    /// <summary>
    /// Le nombre d'éléments par page.
    /// </summary>
    public int PageSize => this.pageSize;

    /// <summary>
    /// L'index du premier élément de la page.
    /// </summary>
    public int StartIndex => this.pageIndex * this.pageSize;

    /// <summary>
    /// L'index après le dernier élément de la page.
    /// </summary>
    public int EndIndex => this.StartIndex + this.pageSize;

    /// <summary>
    /// Crée une nouvelle <see cref="Pagination"/>.
    /// </summary>
    /// <param name="pageIndex">L'index de la page, commençant à zéro.</param>
    /// <param name="pageSize">Le nombre d'éléments par page.</param>
    public Pagination(int pageIndex, int pageSize)
    {
        if (pageIndex < 0) throw new ArgumentOutOfRangeException(nameof(pageIndex), "A page index cannot be negative.");
        this.pageIndex = pageIndex;

        if (pageSize < 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "A page size cannot be negative.");
        this.pageSize = pageSize;
    }

    public Pagination Next() => new(this.pageIndex + 1, this.pageSize);

    public Pagination Previous() => new(this.pageIndex - 1, this.pageSize);
}