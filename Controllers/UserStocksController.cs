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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Stocks_monitoring_system_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStocksController : ControllerBase
    {
        MonitoringStocksContext dbContext = new MonitoringStocksContext();
        // GET: api/<UserStocksController>
        [HttpGet]
        public async Task<ActionResult<UserStock>> Get()
        {
            var userSt = await dbContext.UserStocks.ToListAsync();
            if (userSt == null)

                return NotFound();
            return new ObjectResult(userSt);
        }

        // GET api/<UserStocksController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserStocksController>
        SqlConnection con = new SqlConnection();

        [HttpPost]
        public string Post([FromBody] UserStock value)
        {
            UserStock userSt = new UserStock();
            userSt.IdStocks = value.IdStocks;
            userSt.IdUser = value.IdUser;
            userSt.Amount = value.Amount;
            userSt.Fee = value.Fee;
            userSt.PurchaseDate = value.PurchaseDate;

            try
            {
                //find last price
                StocksChange stch = new StocksChange();
                stch = dbContext.StocksChanges.OrderByDescending(x => x.Date).FirstOrDefault(x => x.IdStocks == userSt.IdStocks);
                //count value price that was payed for this stocks
                if (stch != null)
                {
                    userSt.Value = (userSt.Amount * stch.MarketValue) * (100 + userSt.Fee) / 100;
                }
                //balance update
                //1. find last balance
                double oldBalanceSum = 0;
                Balance balance = dbContext.Balances.OrderByDescending(x => x.Date).FirstOrDefault(x => x.IdUser == userSt.IdUser);
                if (balance != null)
                {
                    oldBalanceSum = balance.CurrentBalance;
                }
                //2. find Name of stocks for msg
                Stock stock = dbContext.Stocks.FirstOrDefault(x => x.Id == userSt.IdStocks);
                //3. update db
                Balance b = new Balance();
                b.IdUser = userSt.IdUser;
                b.CurrentBalance = (double)(oldBalanceSum - userSt.Value);
                b.Date = DateTime.Now;
                b.Msg = "Bought "+ userSt.Amount + " "+ stock.Symbol;
                dbContext.Add(b);
            }
            catch
            {
                userSt.Value = 0;
            }
          

            try
            {
                dbContext.Add(userSt);
                
                dbContext.SaveChanges();
                return JsonConvert.SerializeObject("Stocks was added to user");
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }
        }

        // PUT api/<UserStocksController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserStocksController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserStock>> Delete(int id)
        {
            UserStock userSt = dbContext.UserStocks.FirstOrDefault(x => x.Id == id);
            if (userSt == null)
            {
                return NotFound();
            }
            dbContext.UserStocks.Remove(userSt);
            await dbContext.SaveChangesAsync();
            return Ok(userSt);
        }

    }
}
