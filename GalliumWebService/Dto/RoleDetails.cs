﻿using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using System.ComponentModel.DataAnnotations;

namespace GalliumPlus.WebApi.Dto
{
    public class RoleDetails
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public uint? Permissions { get; set; }

        public RoleDetails()
        {
            this.Id = -1;
            this.Name = String.Empty;
            this.Permissions = null;
        }

        public class Mapper : Mapper<Role, RoleDetails>
        {
            public override RoleDetails FromModel(Role model)
            {
                return new RoleDetails
                {
                    Name = model.Name,
                    Permissions = (uint)model.Permissions,
                    Id = model.Id
                };
            }

            public override Role ToModel(RoleDetails dto)
            {
                return new Role(dto.Id, dto.Name, (Permissions)dto.Permissions!.Value);
            }
        }
    }
}
