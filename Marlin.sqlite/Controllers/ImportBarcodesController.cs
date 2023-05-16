using Marlin.sqlite.Data;
using Marlin.sqlite.Filter;
using Marlin.sqlite.Helper;

using Marlin.sqlite.Models;
using Marlin.sqlite.Services;
using Marlin.sqlite.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Marlin.sqlite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportBarcodesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUriService _uriService;

        public ImportBarcodesController(DataContext context, IUriService uriService)
        {
            _context = context;
            _uriService = uriService;
        }


        [HttpPost]
        public IActionResult ImportData()
        {
            try
            {
                string FilePath = "C:\\Users\\g.kobauri\\source\\repos\\NewRepo3-master\\NewRepo3-master\\Marlin.sqlite\\Barcodes.json";


                string jsonData = System.IO.File.ReadAllText(FilePath);
                JObject data = JObject.Parse(jsonData);
                var tableData = data["json"].ToObject<List<Barcodes>>();




                _context.Barcodes.AddRange(tableData);





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
                var totalCount = _context.Barcodes.Count();

                var data = _context.Barcodes
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var formattedData = data.Select(d => new
                {

                    Field1 = d.AccountId,
                    Field2 = d.ProductID,
                    Field3 = d.Barcode
                    

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

        public async Task<IActionResult> Getcatalog(int id)
        {
            var item = await _context.Catalogues.Where(a => a.ID == id).FirstOrDefaultAsync();
            if (item == null)
            {
                return BadRequest("Item Not Found");
            }
            return Ok(new Response<Catalogues>(item));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Catalogues>> DeleteCatalog(int id)
        {
            var result = await _context.Catalogues
            .FirstOrDefaultAsync(e => e.ID == id);
            if (result != null)
            {
                _context.Catalogues.Remove(result);
                await _context.SaveChangesAsync();
                return result;
            }

            return null;
        }
        [HttpPut]
        public async Task<ActionResult<Catalogues>> UpdateCatalog(Catalogues item)
        {
            var result = await _context.Catalogues
            .FirstOrDefaultAsync(e => e.ID == item.ID);

            if (result != null)
            {
                result.AccountID = item.AccountID;
                result.ProductID = item.ProductID;
                result.SourceCode = item.SourceCode;
                result.Name = item.Name;
                result.Description = item.Description;

                result.Unit = item.Unit;
                result.Status = item.Status;
                result.LastChangeDate = item.LastChangeDate;


                await _context.SaveChangesAsync();

                return result;
            }

            return null;
        }
    }
}
