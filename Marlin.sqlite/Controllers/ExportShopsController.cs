using Marlin.sqlite.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Marlin.sqlite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportShopsController : ControllerBase
    {
        private readonly DataContext _context;

        public ExportShopsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]

        public ActionResult Get()
        {
            var tableContext = _context.Shops.ToList();
            JObject data = new JObject();
            data["Shops"] = JArray.FromObject(tableContext);

            string filePath = "C:/Users/g.kobauri/source/repos/NewRepo3-master/NewRepo3-master/Marlin.sqlite/Shops.json";
            System.IO.File.WriteAllText(filePath, data.ToString());

            return Ok();
        }
    }
}
