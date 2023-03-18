using GalliumPlusAPI.Database;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GalliumPlusAPI.Controllers
{
    public class Controller : ControllerBase
    {
        private static JsonSerializerOptions jsonOptions;
        public static JsonSerializerOptions JsonOptions { set => jsonOptions = value; }

        private IMasterDao dao;
        protected IMasterDao Dao => this.dao;

        public Controller(IMasterDao dao)
        {
            this.dao = dao;
        }

        public static IActionResult Json(object? value, int statusCode = StatusCodes.Status200OK)
        {
            return new ContentResult
            {
                Content = JsonSerializer.Serialize(value, jsonOptions),
                ContentType = "application/json",
                StatusCode = statusCode,
            };
        }

        public static IActionResult Created() => new StatusCodeResult(StatusCodes.Status201Created);
    }
}
