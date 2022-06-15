using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Stocks_monitoring_system_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace Stocks_monitoring_system_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        /*private readonly IConfiguration configuration;

        public  StocksController(IConfiguration configuration)
        {
            //_configuration = configuration;
        }
        */

        MonitoringStocksContext dbContext = new MonitoringStocksContext();

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> Get()
        {
            var stocks = await dbContext.Stocks.ToListAsync();
            if (stocks == null)

                return NotFound();
            return new ObjectResult(stocks);

        }

        [HttpGet("stock/{id}")]
        public async Task<ActionResult<Stock>> Get0(int id)
        {
            int Idstocks = id;
            Stock stocks = await dbContext.Stocks.FirstOrDefaultAsync(x => x.Id == Idstocks);
            if (stocks == null)

                return NotFound();
            return new ObjectResult(stocks);

        }

        SqlConnection con = new SqlConnection();

        [HttpGet("user/{id}")]
        public JsonResult Get1(int id)
        {
            con.ConnectionString = @"Data Source=.;Initial Catalog=MonitoringStocks;Integrated Security=True";
            DataTable table = new DataTable();
            SqlDataReader myReader;
            string query = "Select Stocks.Id, Stocks.Name, Stocks.Company_name, stocks.symbol, stocks.market_scope, "+
            "stocks.type, stocks.behavior from Stocks, user_stocks where Stocks.Id = user_stocks.Id_stocks " +
            "and Id_user = " + id +
            " group by stocks.Id, Stocks.Name, Stocks.Company_name, stocks.symbol, stocks.market_scope, stocks.type, stocks.behavior";
            using (SqlConnection sqlCon = new SqlConnection(con.ConnectionString))
            {
                sqlCon.Open();
                

                using (SqlCommand  sqlcmd = new SqlCommand(query, sqlCon))
                {
                    myReader = sqlcmd.ExecuteReader();
                    table.Load(myReader); 
                    myReader.Close();
                    sqlCon.Close();
                }
            }
            
            return new JsonResult(table);

        }
        

        [HttpPost]
        public string Post([FromBody] Stock value)
        {
            Stock stock = new Stock();
            stock.Name = value.Name;
            stock.CompanyName = value.CompanyName;
            stock.Symbol = value.Symbol;
            stock.MarketScope = value.MarketScope;
            stock.Type = value.Type;
            stock.Info = value.Info;
            stock.NetIncome = value.NetIncome;
            stock.WeightedAverage = value.WeightedAverage;
         

            try
            {
                dbContext.Add(stock);
                dbContext.SaveChanges();
                return JsonConvert.SerializeObject("New stock was created");
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }
        }

        // PUT api/<StockController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Stock>> Put( Stock stock)
        {
            //
            int id = stock.Id;
            //
            if (stock == null)
            {
                return BadRequest();
            }
            if (!dbContext.Stocks.Any(x => x.Id == id))
            {
                return NotFound();
            }

            //stock.Id = id;

            dbContext.Update(stock);
            await dbContext.SaveChangesAsync();
            return Ok(stock);
        }

        // DELETE api/<StocksController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Stock>> Delete(int id)
        {
            Stock stock = dbContext.Stocks.FirstOrDefault(x => x.Id == id);
            if (stock == null)
            {
                return NotFound();
            }
            dbContext.Stocks.Remove(stock);
            await dbContext.SaveChangesAsync();
            return Ok(stock);
        }

        //select * from stocks where [type] = 'Common stock' and market_scope = ''
        [HttpPut("Similar")]
        public async Task<ActionResult<IEnumerable<Stock>>> PutSimilarList(Stock stock)
        {
            string marketscope = stock.MarketScope;
            string type = stock.Type;
            int id = stock.Id;

            if (stock == null)
            {
                return BadRequest();
            }
            var stocks = await dbContext.Stocks.Where(x => x.MarketScope == marketscope 
            && x.Type == type && x.Id != id ).ToListAsync();
            if (stocks == null)
                return NotFound();
            return new ObjectResult(stocks);
        }

        [HttpGet("Similar/{id}")]
        public async Task<ActionResult<IEnumerable<Stock>>> getSimilarList(int id)
        {
            int Idstocks = id;
            Stock stock = await dbContext.Stocks.FirstOrDefaultAsync(x => x.Id == Idstocks);
            string marketscope = stock.MarketScope;
            string type = stock.Type;

            if (stock == null)
            {
                return BadRequest();
            }
            var stocks = await dbContext.Stocks.Where(x => x.MarketScope == marketscope && x.Type == type && x.Id != id).ToListAsync();
            if (stocks == null)
                return NotFound();
            return new ObjectResult(stocks);
        }
    }
}
