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
    public class StocksChangeController : ControllerBase
    {
        MonitoringStocksContext dbContext = new MonitoringStocksContext();
        
        
        // GET: api/<StocksChangeController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<StocksChangeController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StocksChange>> Get(int id)
        {
            StocksChange stockChange = await dbContext.StocksChanges.FirstOrDefaultAsync(x => x.Id == id);
            if (stockChange == null)
                return NotFound();
            return new ObjectResult(stockChange);
        }

        [HttpGet("stock/{id}")]
        public async Task<ActionResult<IEnumerable<StocksChange>>> Get2(int id)
        {
            var stchanges = await dbContext.StocksChanges.OrderBy(x => x.Date).Where(x => x.IdStocks == id).ToListAsync();
            if (stchanges == null)
                return NotFound();

            return new ObjectResult(stchanges);
        }

        SqlConnection con = new SqlConnection();

        // POST api/<StocksChangeController>
        [HttpPost]
        public async Task<string> PostAsync([FromBody] StocksChange value)
        {
            StocksChange sch = new StocksChange();
            sch.IdStocks = value.IdStocks;
            int id = sch.IdStocks;
            sch.Weight = value.Weight;
            sch.MarketValue = value.MarketValue;
            sch.StandardDeviation = value.StandardDeviation;
            sch.Date = value.Date;

            double previousValue = FindLastMarketValueStock(value.IdStocks);
            //update behavior of the stock (raising or falling)
            if(previousValue > sch.MarketValue)
            {
                await UpdateBehavior(id, "Falling");
            }
            else
            {
                await UpdateBehavior(id, "Raising");
            }

            try
            {
                dbContext.Add(sch);
                dbContext.SaveChanges();

                //send notifications to client
                con.ConnectionString = @"Data Source=.;Initial Catalog=MonitoringStocks;Integrated Security=True";
                DataTable table = new DataTable();
                SqlDataReader myReader;
                string query = "Select distinct Id_user, [Name]  from user_stocks, stocks where " +
                "user_stocks.Id_stocks = stocks.Id and Id_stocks = " + sch.IdStocks;

                //List<int> usersId = new List<int>();
                string name = "";
                using (SqlConnection sqlCon = new SqlConnection(con.ConnectionString))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlcmd = new SqlCommand(query, sqlCon))
                    {
                        myReader = sqlcmd.ExecuteReader();
                        table.Load(myReader);
                        int numberofusers = table.Rows.Count;
                        int[] usersId = new int[numberofusers];
                        int countRows = 0;
                        foreach (DataRow row in table.Rows)
                        {
                            usersId[countRows] = int.Parse(row["Id_user"].ToString());
                            countRows++;
                            name = row["Name"].ToString();
                        }
                        myReader.Close();
                        sqlCon.Close();

                        //create msg for each user from list
                        for (int i = 0; i < countRows; i++ )
                        {
                            Notification notif = new Notification();
                            notif.IdUser = usersId[i];
                            if (previousValue != value.MarketValue)
                            {
                                if(previousValue > value.MarketValue)
                                {
                                    double res = previousValue - value.MarketValue;
                                    notif.Text = name + "  prices dropped by  " + res + ".\n" +
                                         "New market value price is " + value.MarketValue;
                                    notif.Type = "fall";

                                    
                                }
                                else
                                {
                                    double res = value.MarketValue - previousValue;
                                    notif.Text = name + "  prices rised by  " + res + ".\n" +
                                         "New market value price is " + value.MarketValue;
                                    notif.Type = "rise";

                                }
                                notif.Checked = false;
                                dbContext.Add(notif);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
          

                return JsonConvert.SerializeObject("new stock change was added. ");
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }
        }

        private double FindLastMarketValueStock(int id)
        {
            double mv = 0;
            con.ConnectionString = @"Data Source=.;Initial Catalog=MonitoringStocks;Integrated Security=True";
            DataTable table = new DataTable();
            SqlDataReader myReader;
            string query = "Select top 1 market_value,Stocks.* , [date] from Stocks, Stocks_change where Stocks.Id = Stocks_change.Id_stocks and stocks.Id=" + id + " order by[date] DESC";
            using (SqlConnection sqlCon = new SqlConnection(con.ConnectionString))
            {
                sqlCon.Open();
                using (SqlCommand sqlcmd = new SqlCommand(query, sqlCon))
                {
                    myReader = sqlcmd.ExecuteReader();
                    table.Load(myReader);
                    foreach (DataRow row in table.Rows)
                    {
                        mv = double.Parse(row["market_value"].ToString());
                    }
                    myReader.Close();
                    sqlCon.Close();
                }
            }
            return mv;
        }

        private async Task<ActionResult<Stock>> UpdateBehavior(int id, string behavior)
        {
            Stock stock = await dbContext.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (stock== null)
                return NotFound();
            stock.Behavior = behavior;

            dbContext.Update(stock);
            await dbContext.SaveChangesAsync();
            return new ObjectResult(stock);
        }


        // PUT api/<StocksChangeController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<StocksChange>> Put(int id, StocksChange stch)
        {
            if (stch == null)
            {
                return BadRequest();
            }
            if (!dbContext.StocksChanges.Any(x => x.Id == id))
            {
                return NotFound();
            }

            stch.Id = id;

            dbContext.Update(stch);
            await dbContext.SaveChangesAsync();
            return Ok(stch);
        }

        // DELETE api/<StocksChangeController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<StocksChange>> Delete(int id)
        {
            StocksChange stch = dbContext.StocksChanges.FirstOrDefault(x => x.Id == id);
            if (stch == null)
            {
                return NotFound();
            }
            dbContext.StocksChanges.Remove(stch);
            await dbContext.SaveChangesAsync();
            return Ok(stch);
        }

        [HttpPost("populate")]
        public string Populate([FromBody] StocksChange value)
        {
            StocksChange sch = new StocksChange();
            sch.IdStocks = value.IdStocks;
            sch.Weight = value.Weight;
            sch.MarketValue = value.MarketValue;
            sch.StandardDeviation = value.StandardDeviation;
            sch.Date = value.Date;


            try
            {
                dbContext.Add(sch);
                dbContext.SaveChanges();
                return JsonConvert.SerializeObject("new stock change was added");
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }
        }


    }
}
