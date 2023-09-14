using GalliumPlus.WebApi.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Exceptions
{
    /// <summary>
    /// Erreur indiquant que l'utilisateur n'a pas les permissions suffisantes
    /// pour effectuer une action.
    /// </summary>
    public class PermissionDeniedException : GalliumException
    {
        private Permissions required;

        /// <summary>
        /// Les permissions qu étaient requises.
        /// </summary>
        public Permissions Required { get => required; }

        public override ErrorCode ErrorCode => ErrorCode.PERMISSION_DENIED;
        /// <summary>
        /// Instancie l'exception.
        /// </summary>
        /// <param name="required">Les permissions requises pour effectuer l'action.</param>
        public PermissionDeniedException(Permissions required) : base()
        {
            this.required = required;
        }
    }
}
