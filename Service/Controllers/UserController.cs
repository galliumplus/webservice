using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{

    [Route("api/users")]
    [ApiController]
    public class UserController : Controller
    {
        private IMapper<User, UserSummary> mapper;

        public UserController(IMasterDao dao, IMapper<User, UserSummary> mapper)
        : base(dao)
        {
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(mapper.FromModel(Dao.Users.Read()));
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            return Json(mapper.FromModel(Dao.Users.Read(id)));
        }

        [HttpPost]
        public IActionResult Post(UserSummary newUser)
        {
            Dao.Users.Create(mapper.ToModel(newUser, Dao));
            return Created();
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, UserSummary updatedUser)
        {
            Dao.Users.Update(id, mapper.ToModel(updatedUser, Dao));
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            Dao.Users.Delete(id);
            return Ok();
        }

        [HttpPut("{id}/deposit")]
        public IActionResult PutDeposit(string id, [FromBody] double updatedDeposit)
        {
            Dao.Users.UpdateDeposit(id, updatedDeposit);
            return Ok();
        }
    }
}
