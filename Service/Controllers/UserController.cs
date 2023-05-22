using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto;
using GalliumPlus.WebApi.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{

    [Route("api/users")]
    [Authorize]
    [ApiController]
    public class UserController : Controller
    {
        private IUserDao userDao;
        private UserSummary.Mapper summaryMapper = new();
        private UserDetails.Mapper detailsMapper = new();

        public UserController(IUserDao userDao)
        {
            this.userDao = userDao;
        }

        [HttpGet]
        [RequiresPermissions(Permissions.SEE_ALL_USERS_AND_ROLES)]
        public IActionResult Get()
        {
            return Json(this.summaryMapper.FromModel(this.userDao.Read()));
        }

        [HttpGet("{id}", Name = "user")]
        public IActionResult Get(string id)
        {
            if (id != this.User!.Id)
            {
                RequirePermissions(Permissions.SEE_ALL_USERS_AND_ROLES);
            }
            return Json(this.detailsMapper.FromModel(this.userDao.Read(id)));
        }

        [HttpPost]
        [RequiresPermissions(Permissions.MANAGE_USERS)]
        public IActionResult Post(UserSummary newUser)
        {
            User user = this.userDao.Create(this.summaryMapper.ToModel(newUser, this.userDao));
            return Created("user", user.Id, this.detailsMapper.FromModel(user));
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permissions.MANAGE_USERS)]
        public IActionResult Put(string id, UserSummary updatedUser)
        {
            this.userDao.Update(id, this.summaryMapper.ToModel(updatedUser, this.userDao));
            return Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permissions.MANAGE_USERS)]
        public IActionResult Delete(string id)
        {
            this.userDao.Delete(id);
            return Ok();
        }

        [HttpPut("{id}/deposit")]
        [RequiresPermissions(Permissions.MANAGE_DEPOSITS)]
        public IActionResult PutDeposit(string id, [FromBody] double updatedDeposit)
        {
            this.userDao.UpdateDeposit(id, updatedDeposit);
            return Ok();
        }
    }
}
