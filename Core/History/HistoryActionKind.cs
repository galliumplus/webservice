using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.History
{
    public enum HistoryActionKind
    {
        LOG_IN = 1,
        EDIT_PRODUCT_OR_CATEGORIES = 2,
        EDIT_USERS_OR_ROLES = 3,
        PURCHASE = 4,
    }
}
