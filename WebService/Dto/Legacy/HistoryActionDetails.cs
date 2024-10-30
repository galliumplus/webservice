using System.Text.Json.Serialization;
using GalliumPlus.Core.Logs;

namespace GalliumPlus.WebService.Dto.Legacy
{
    public class HistoryActionDetails
    {
        public HistoryActionKind ActionKind { get; set; }

        public string Text { get; set; } = string.Empty;

        public DateTime Time { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Actor { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Target { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? NumericValue { get; set; }

        public class Mapper : Mapper<HistoryAction, HistoryActionDetails>
        {
            public override HistoryActionDetails FromModel(HistoryAction model)
            {
                return new HistoryActionDetails
                {
                    ActionKind = model.ActionKind,
                    Text = model.Text,
                    Time = model.Time,
                    Actor = model.Actor,
                    Target = model.Target,
                    NumericValue = model.NumericValue
                };
            }

            public override HistoryAction ToModel(HistoryActionDetails dto)
            {
                throw new InvalidOperationException("Les données de l'historique ne peuvent pas entrer.");
            }
        }
    }
}
