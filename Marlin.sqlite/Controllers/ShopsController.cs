using Marlin.sqlite.Data;
using Marlin.sqlite.Filter;
using Marlin.sqlite.Wrappers;
using Marlin.sqlite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Marlin.sqlite.Services;
using Marlin.sqlite.Helper;
using Newtonsoft.Json.Linq;

namespace Marlin.sqlite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUriService _uriService;

        public ShopsController(DataContext context, IUriService uriService)
        {
            _context = context;
            _uriService = uriService;
        }
        [HttpPost]
        public IActionResult ImportData()
        {
            try
            {
                string FilePath = "C:\\Users\\g.kobauri\\source\\repos\\NewRepo3-master\\NewRepo3-master\\Marlin.sqlite\\Shops.json";


                string jsonData = System.IO.File.ReadAllText(FilePath);
                JObject data = JObject.Parse(jsonData);
                var tableData = data["json"].ToObject<List<Shops>>();




                _context.Shops.AddRange(tableData);





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
                var totalCount = _context.Shops.Count();

                var data = _context.Shops
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var formattedData = data.Select(d => new
                {

                    AccountID = d.AccountID,
                    ShopID = d.ShopID,
                    SourceCode = d.SourceCode,
                    Name = d.Name,
                    Description = d.Description,
                    Address = d.Address,
                    ContactPerson = d.ContactPerson,
                    ContactNumber = d.ContactNumber,
                    Email = d.Email,
                    Region = d.Region,
                    Format = d.Format,
                    GPS = d.GPS

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

        [HttpGet("{id}")]

        public async Task<IActionResult> GetShop(int id)
        {
            var user = await _context.Shops.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            return Ok(new Response<Shops>(user));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Shops>> DeleteShops(int id)
        {
            var result = await _context.Shops
            .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                _context.Shops.Remove(result);
                await _context.SaveChangesAsync();
                return result;
            }

            return null;
        }
        [HttpPut]
        public async Task<ActionResult<Shops>> UpdateShops(Shops item)
        {
            var result = await _context.Shops
            .FirstOrDefaultAsync(e => e.Id == item.Id);

            if (result != null)
            {
                result.AccountID = item.AccountID;
                result.ShopID = item.ShopID;
                result.SourceCode = item.SourceCode;
                result.Name = item.Name;
                result.Description = item.Description;
                result.Address = item.Address;
                result.ContactPerson = item.ContactPerson;
                result.ContactNumber = item.ContactNumber;
                result.Email = item.Email;
                result.Region = item.Region;
                result.Format = item.Format;
                result.GPS = item.GPS;




                await _context.SaveChangesAsync();

                return result;
            }

            return null;
        }
    }
}
