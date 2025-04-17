using GalliumPlus.Core.Data;
using GalliumPlus.Core.Data.LogsSearch;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Security;
using GalliumPlus.WebService.Dto.Legacy;
using GalliumPlus.WebService.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers;

[Route("v1")]
[Authorize]
[ApiController]
public class LogsController(ILogsDao logsDao, IHistoryDao historyDao) : GalliumController
{
    private readonly HistoryActionDetails.Mapper mapper = new();

    private static DateTime ParseTime(string dateTime, string queryParameter)
    {
        try
        {
            return DateTime.Parse(dateTime);
        }
        catch (FormatException)
        {
            throw new InvalidResourceException($"Format de date invalide pour le paramètre {queryParameter}");
        }
    }
    
    [HttpGet("logs")]
    [RequiresPermissions(Permissions.READ_LOGS)]
    public IActionResult GetLogs(
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

        return this.Json(logsDao.Read(criteria, pagination));
    }

    [HttpGet("history")]
    [RequiresPermissions(Permissions.READ_LOGS)]
    public IActionResult GetLegacyHistory(
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

        return this.Json(this.mapper.FromModel(historyDao.Read(criteria, pagination)));
    }
}