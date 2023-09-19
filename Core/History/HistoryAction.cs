using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.History
{
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
