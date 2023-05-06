using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Dto
{
    public class RoleDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public uint Permissions { get; set; }

        [JsonConstructor]
        public RoleDetails(string name, uint permissions, int id = -1)
        {
            this.Id = id;
            this.Name = name;
            this.Permissions = permissions;
        }

        public class Mapper : Mapper<Role, RoleDetails>
        {
            public override RoleDetails FromModel(Role model)
            {
                return new RoleDetails(model.Name, (uint)model.Permissions, model.Id);
            }

            public override Role ToModel(RoleDetails dto, IMasterDao dao)
            {
                return new Role(dto.Id, dto.Name, (Permissions)dto.Permissions);
            }
        }
    }
}
