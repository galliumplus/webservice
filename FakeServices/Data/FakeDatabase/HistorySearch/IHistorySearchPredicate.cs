using GalliumPlus.WebApi.Core.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Data.FakeDatabase.HistorySearch
{
    internal interface IHistorySearchPredicate
    {
        bool Matches(HistoryAction action);
    }
}
