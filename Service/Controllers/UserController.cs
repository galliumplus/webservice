using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto;
using GalliumPlus.WebApi.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{

    [Route("api/users")]
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
        public IActionResult Get()
        {
            return Json(this.summaryMapper.FromModel(this.userDao.Read()));
        }

        [HttpGet("{id}", Name = "user")]
        public IActionResult Get(string id)
        {
            return Json(this.detailsMapper.FromModel(this.userDao.Read(id)));
        }

        [HttpPost]
        public IActionResult Post(UserSummary newUser)
        {
            User user = this.userDao.Create(this.summaryMapper.ToModel(newUser, this.userDao));
            return Created("user", user.Id, this.detailsMapper.FromModel(user));
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, UserSummary updatedUser)
        {
            this.userDao.Update(id, this.summaryMapper.ToModel(updatedUser, this.userDao));
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            this.userDao.Delete(id);
            return Ok();
        }

        [HttpPut("{id}/deposit")]
        public IActionResult PutDeposit(string id, [FromBody] double updatedDeposit)
        {
            this.userDao.UpdateDeposit(id, updatedDeposit);
            return Ok();
        }
    }
}
