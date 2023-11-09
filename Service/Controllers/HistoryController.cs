using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Data.HistorySearch;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("api/history")]
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

            return Json(this.mapper.FromModel(this.historyDao.Read(criteria, pagination)));
        }
    }
}
