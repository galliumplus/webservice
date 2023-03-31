using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{

    [Route("api/users")]
    [ApiController]
    public class UserController : Controller
    {
        public UserController(IMasterDao dao) : base(dao) { }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(Dao.Users.Read());
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                return Json(Dao.Users.Read(id));
            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Post(User newUser)
        {
            Dao.Users.Create(newUser);
            return Created();
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, User updatedUser)
        {
            try
            {
                Dao.Users.Update(id, updatedUser);
                return Ok();
            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                Dao.Users.Delete(id);
                return Ok();
            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/deposit")]
        public IActionResult PutDeposit(string id, [FromBody] double updatedDeposit)
        {
            try
            {
                Dao.Users.UpdateDeposit(id, updatedDeposit);
                return Ok();
            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
