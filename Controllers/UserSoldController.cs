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
    public class UserSoldController : ControllerBase
    {
        MonitoringStocksContext dbContext = new MonitoringStocksContext();
        // GET: api/<UserSoldController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserSoldController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserSoldController>
        [HttpPost]
        public string Post([FromBody] UserSold value)
        {
            UserSold userSold = new UserSold();
            userSold.IdStocks = value.IdStocks;
            userSold.IdUser = value.IdUser;
            userSold.Amount = value.Amount ;
            userSold.Fee = value.Fee;
            userSold.SellingDate = DateTime.Now;

            try
            {
                //find last price
                StocksChange stch = new StocksChange();
                stch = dbContext.StocksChanges.OrderByDescending(x => x.Date).FirstOrDefault(x => x.IdStocks == userSold.IdStocks);
                //count value with price
                userSold.Value = (userSold.Amount * stch.MarketValue) * (100 - userSold.Fee) / 100 ;

                //balance update
                //1. find last balance
                double oldBalanceSum = 0;
                Balance balance = dbContext.Balances.OrderByDescending(x => x.Date).FirstOrDefault(x => x.IdUser == userSold.IdUser);
                if (balance != null)
                {
                    oldBalanceSum = balance.CurrentBalance;
                }
                //2. find Name of stocks for msg
                Stock stock = dbContext.Stocks.FirstOrDefault(x => x.Id == userSold.IdStocks);
                //3. update db
                Balance b = new Balance();
                b.IdUser = userSold.IdUser;
                b.CurrentBalance = (double)(oldBalanceSum + userSold.Value);
                b.Date = DateTime.Now;
                b.Msg = "Sold " + userSold.Amount + " " + stock.Symbol;
                dbContext.Add(b);
            }
            catch
            {
                userSold.Value = 0;
            }


            try
            {
                dbContext.Add(userSold);
                dbContext.SaveChanges();
                return JsonConvert.SerializeObject("Stocks were sold");
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }
        }

        // PUT api/<UserSoldController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserSoldController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
