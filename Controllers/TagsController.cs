using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stocks_monitoring_system_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace Stocks_monitoring_system_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        MonitoringStocksContext dbContext = new MonitoringStocksContext();

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StocksTag>>> Get()
        {
            var tags = await dbContext.StocksTags.ToListAsync();
            if (tags == null)

                return NotFound();
            return new ObjectResult(tags);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StocksTag>> Get0(int id)
        {
            StocksTag stocktag = await dbContext.StocksTags.FirstOrDefaultAsync(x => x.Id == id);
            if (stocktag == null)
                return NotFound();
            return new ObjectResult(stocktag);
        }

        [HttpGet("stock/{id}")]
        public async Task<ActionResult<IEnumerable<StocksTag>>> Get2(int id)
        {
            var stocktags = await dbContext.StocksTags.Where(x => x.IdStocks == id).ToListAsync();
            if (stocktags == null)
                return NotFound();

            return new ObjectResult(stocktags);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<StocksTag>> Delete(int id)
        {
            StocksTag stockstag = dbContext.StocksTags.FirstOrDefault(x => x.Id == id);
            if (stockstag == null)
            {
                return NotFound();
            }
            dbContext.StocksTags.Remove(stockstag);
            await dbContext.SaveChangesAsync();
            return Ok(stockstag);
        }

        [HttpPost]
        public string Post([FromBody] StocksTag value)
        {
            StocksTag tag = new StocksTag();
            tag.IdStocks = value.IdStocks;
            tag.Tag = value.Tag;
            tag.Type = value.Type;
            tag.Value = value.Value;
            


            try
            {
                dbContext.Add(tag);
                dbContext.SaveChanges();
                return JsonConvert.SerializeObject("new tag was added to the stock");
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }
        }

    }
}
