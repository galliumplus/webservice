using GalliumPlusAPI.Database;
using GalliumPlusAPI.Database.Criteria;
using GalliumPlusAPI.Exceptions;
using GalliumPlusAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlusAPI.Controllers
{
    [Route("api/bundles")]
    [ApiController]
    public class BundleController : Controller
    {
        public BundleController(IMasterDao dao) : base(dao) { }

        [HttpGet]
        public IActionResult Get(bool availableOnly = true)
        {
            return Json(Dao.Bundles.FindAll(
                new BundleCriteria { AvailableOnly = availableOnly }
            ));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                return Json(Dao.Bundles.ReadOne(id));
            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Post(Bundle newBundle)
        {
            Dao.Bundles.Create(newBundle);
            return Created();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Bundle updatedBundle)
        {
            try
            {
                Dao.Bundles.Update(id, updatedBundle);
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
                Dao.Bundles.Delete(id);
                return Ok();
            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
