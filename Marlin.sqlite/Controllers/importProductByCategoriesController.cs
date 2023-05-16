using Marlin.sqlite.Data;
using Marlin.sqlite.Models;
using Marlin.sqlite.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Marlin.sqlite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class importProductByCategoriesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUriService _uriService;

        public importProductByCategoriesController(DataContext context, IUriService uriService)
        {
            _context = context;
           
            _uriService = uriService;
        }


        [HttpPost]
        public IActionResult ImportData()
        {
            try
            {
                string FilePath = "C:\\Users\\g.kobauri\\source\\repos\\NewRepo3-master\\NewRepo3-master\\Marlin.sqlite\\ProductsByCategories.json";


                string jsonData = System.IO.File.ReadAllText(FilePath);
                JObject data = JObject.Parse(jsonData);
                var tableData = data["json"].ToObject<List<ProductsByCategories>>();




                _context.ProductsByCategories.AddRange(tableData);





                _context.SaveChanges();


                return Ok(new { message = "Data imported successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }


        [HttpGet]

        public IActionResult GetData(int page = 1, int pageSize = 10)
        {
            try
            {
                var totalCount = _context.ProductsByCategories.Count();

                var data = _context.ProductsByCategories
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var formattedData = data.Select(d => new
                {

                    AccountID = d.AccountID,
                    productid = d.ProductID,
                    categoryid = d.CategoryID
                    

                    // Add more fields as necessary, following the same pattern
                });

                var response = new
                {
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    Data = formattedData,
                    PreviousPage = page > 1 ? Url.Action("GetData", new { page = page - 1, pageSize }) : null,
                    NextPage = page < (totalCount + pageSize - 1) / pageSize ? Url.Action("GetData", new { page = page + 1, pageSize }) : null
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
