using GalliumPlus.Core.Data;
using GalliumPlus.Core.Data.HistorySearch;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto;
using GalliumPlus.WebService.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers
{
    [Route("v1/history")]
    [Authorize]
    [ApiController]
    public class HistoryController : Controller
    {
        private IHistoryDao historyDao;
        private HistoryActionDetails.Mapper mapper;

        public HistoryController(IHistoryDao historyDao)
        {
            this.historyDao = historyDao;
            this.mapper = new();
        }

        private static DateTime ParseTime(string dateTime, string queryParameter)
        {
            try
            {
                return DateTime.Parse(dateTime);
            }
            catch (FormatException)
            {
                throw new InvalidItemException($"Format de date invalide pour le paramètre {queryParameter}");
            }
        }

        [HttpGet]
        [RequiresPermissions(Permissions.READ_LOGS)]
        public IActionResult Get(
            int pageSize = 50,
            int pageIndex = 0,
            string? from = null,
            string? to = null
        )
        {
            Pagination pagination = new(pageIndex, pageSize);

            AndCriteria criteria = new();
            if (from != null) criteria.Add(new FromCriteria(ParseTime(from, nameof(from))));
            if (to != null) criteria.Add(new ToCriteria(ParseTime(to, nameof(to))));

            return this.Json(this.mapper.FromModel(this.historyDao.Read(criteria, pagination)));
        }
    }
}
