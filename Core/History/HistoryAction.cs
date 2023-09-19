namespace GalliumPlus.WebApi.Core.History
{
    /// <summary>
    /// Représente une entrée de l'historique.
    /// </summary>
    public class HistoryAction
    {
        private HistoryActionKind actionKind;
        private string text;
        private string? actor, target;
        private decimal? numericValue;

        public HistoryAction(
            HistoryActionKind actionKind,
            string text,
            string? actor = null,
            string? target = null,
            decimal? numericValue = null
        )
        {
            this.actionKind = actionKind;
            this.text = text;
            this.actor = actor;
            this.target = target;
            this.numericValue = numericValue;
        }
    }
}
