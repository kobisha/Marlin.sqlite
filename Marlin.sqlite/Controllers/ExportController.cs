using Marlin.sqlite.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Marlin.sqlite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly DataContext _context;

        public ExportController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Get()
        {
            // Retrieve data from Table1
            var table1Data = _context.OrderHeaders.ToList();

            // Retrieve data from Table2
            var table2Data = _context.OrderDetails.ToList();

            // Create a JObject to hold the data for both tables
            JObject data = new JObject();
            data["Headers"] = JArray.FromObject(table1Data);
            data["Details"] = JArray.FromObject(table2Data);

            // Write data to a local JSON file
            string filePath = "C:/Users/g.kobauri/source/repos/NewRepo3-master/NewRepo3-master/Marlin.sqlite/ALLOrders.json";
            System.IO.File.WriteAllText(filePath, data.ToString());

            return Ok();
        }
    }
    
}
