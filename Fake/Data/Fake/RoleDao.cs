﻿using GalliumPlus.Core.Data;
using GalliumPlus.Core.Users;

namespace GalliumPlus.Data.Fake
{
    public class RoleDao : BaseDaoWithAutoIncrement<Role>, IRoleDao
    {
        public RoleDao()
        {
            this.Create(
                new Role(0, "Adhérent", Permissions.NONE)
            );
            this.Create(
                new Role(0, "CA",
                    Permissions.MANAGE_PRODUCTS
                    | Permissions.SEE_ALL_USERS_AND_ROLES
                    | Permissions.SELL
                    | Permissions.MANAGE_DEPOSITS
                )
            );
            this.Create(
                new Role(0, "Président",
                    Permissions.MANAGE_PRODUCTS
                    | Permissions.SEE_ALL_USERS_AND_ROLES
                    | Permissions.SELL
                    | Permissions.MANAGE_CATEGORIES
                    | Permissions.READ_LOGS
                    | Permissions.MANAGE_ROLES
                    | Permissions.MANAGE_USERS
                    | Permissions.RESET_MEMBERSHIPS
                    | Permissions.MANAGE_CLIENTS
                )
            );
        }

        protected override int GetKey(Role item) => item.Id;
        protected override void SetKey(ref Role item, int key) => item.Id = key;
    }
}