using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Exceptions;

public class FailedPreconditionException : GalliumException
{
    public override ErrorCode ErrorCode => ErrorCode.FailedPrecondition;

    public FailedPreconditionException(string message) : base(message) { }
}