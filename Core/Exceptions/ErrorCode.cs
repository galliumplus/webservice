using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Exceptions
{
    public enum ErrorCode
    {
        CANT_SELL,
        DUPLICATE_ITEM,
        INVALID_ITEM,
        ITEM_NOT_FOUND,
        PERMISSION_DENIED
    }
}
