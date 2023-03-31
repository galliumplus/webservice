using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : Controller
    {
        public RoleController(IMasterDao dao) : base(dao) { }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(Dao.Roles.Read());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                return Json(Dao.Roles.Read(id));
            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Post(Role newRole)
        {
            Dao.Roles.Create(newRole);
            return Created();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Role updatedRole)
        {
            try
            {
                Dao.Roles.Update(id, updatedRole);
                return Ok();
            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Dao.Roles.Delete(id);
                return Ok();
            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
