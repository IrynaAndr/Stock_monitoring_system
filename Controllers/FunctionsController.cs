using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public class FunctionsController : ControllerBase
    {

        MonitoringStocksContext dbContext = new MonitoringStocksContext();

        [HttpGet("date")]
        public string GetTimeNow()
        {
            //DateTime thisDay = DateTime.Today;
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            return JsonConvert.SerializeObject(date);
        }

        SqlConnection con = new SqlConnection();
        private DataTable Sql(string query)
        {
            con.ConnectionString = @"Data Source=.;Initial Catalog=MonitoringStocks;Integrated Security=True";
            DataTable table = new DataTable();
            SqlDataReader myReader;
            // query = "Select Company_name, Symbol from Stocks";
            using (SqlConnection sqlCon = new SqlConnection(con.ConnectionString))
            {
                sqlCon.Open();

                //int result = (int)sqlcmd.ExecuteScalar();
                //farmer.IdUser = result;

                using (SqlCommand sqlcmd = new SqlCommand(query, sqlCon))
                {
                    myReader = sqlcmd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    sqlCon.Close();
                }
            }
            return table;
        }
        private JsonResult SqlQueryTable(string query)
        {
            DataTable table = Sql(query);
            return new JsonResult(table);
        }
        private string SqlQueryString(string query)
        {
            DataTable table = Sql(query);
            return JsonConvert.SerializeObject(table);
        }


        [HttpGet("StockCountUsers/{id}")]
        public string GetCountUsers(int id)
        {
            string res = SqlQueryString("");
            return res;
        }

        [HttpGet("StockValue/{id}")]
        public JsonResult GetValue(int id)
        {
            JsonResult res = SqlQueryTable("Select top 1 market_value,standard_deviation, Stocks.* , [date] from Stocks, Stocks_change where Stocks.Id = Stocks_change.Id_stocks and stocks.Id=" + id + " order by[date] DESC");
            return res;
        }

        [HttpGet("StockExpectedReturn/{id}")]
        public double GetExpReturn(int id, Stock stock)
        {
            double NetIncome = (double)stock.NetIncome;
            double WeightedAverage = (double)stock.NetIncome;

            string current_price = SqlQueryString("Select top 1 market_value,Stocks.* , [date] " +
                "from Stocks, Stocks_change where Stocks.Id = Stocks_change.Id_stocks "+
                "and stocks.Id=" + stock.Id + " order by[date] DESC");
            double price;
            double expected_return = 0;
            if (Double.TryParse(current_price, out price))
            {
                expected_return = Math.Round(NetIncome / (WeightedAverage * price), 2);
            }
            return expected_return;
        }

        [HttpGet("TypesStock")]
        public JsonResult GetTypes()
        {
            JsonResult res = SqlQueryTable("Select Distinct [Type] from Stocks");
            return res;
        }

        [HttpGet("History/{IdUser}/{IdStocks}")]
        public JsonResult GetHistory(int IdUser, int IdStocks)
        {
            string query = "Select Amount, [Value], purchase_date as [Date], 'bought' as [Event] from user_stocks " +
            "where Id_user = " + IdUser + " and Id_stocks = " + IdStocks +
            " Union " +
            "Select Amount, [Value], selling_date as [Date], 'sold' as [Event] from user_solds " +
            "where Id_user = " + IdUser + " and Id_stocks = " + IdStocks +
            " order by [Date]";
            JsonResult res = SqlQueryTable(query);
            return res;
        }
        [HttpGet("TotalBought/{IdUser}/{IdStocks}")]
        public string GetStocksAmountBought(int IdUser, int IdStocks)
        {
            string query = "select 'Total bought' as Res, sum(Amount) as Amount, sum(Value) as [Value] from User_stocks "+
            "where Id_user = " + IdUser + " and Id_stocks = " + IdStocks +
            " Union " +
            "select 'Total sold' as Res, sum(Amount)  as Amount, sum(Value)  as [Value] from User_solds " +
            "where Id_user = " + IdUser + " and Id_stocks = " + IdStocks ;
            string res = SqlQueryString(query);
            return res;
        }

        [HttpGet("StockMin/{id}")]
        public JsonResult GetMin(int id)
        {
            JsonResult res = SqlQueryTable(
                "SELECT market_value, [date] from stocks_change a where market_value = ( "
                +"Select min(market_value) from stocks_change b where b.Id_stocks = " + id +")");
            return res;
        }
        [HttpGet("StockMax/{id}")]
        public JsonResult GetMax(int id)
        {
            JsonResult res = SqlQueryTable(
               "SELECT market_value, [date] from stocks_change a where market_value = ( "
               + "Select max(market_value) from stocks_change b where b.Id_stocks = " + id + ")");
            return res;
        }

        [HttpGet("StockAvg/{id}")]
        public JsonResult GetAvg(int id)
        {
            JsonResult res = SqlQueryTable("Select  AVG(market_value) as average from stocks_change where Id_stocks = " + id );
            return res;
        }

        [HttpGet("NetSales/{id}")]
        public JsonResult GetNetSales(int id)
        {
            JsonResult res = SqlQueryTable("select sum([value]) from user_solds where Id_stocks = " + id);
            return res;
        }


        [HttpGet("PopularityList")]
        public JsonResult GetPopularityList()
        {
            string query = "select Id_stocks,Count(DISTINCT Id_user)as UsersCount " +
            "from user_stocks Group by Id_stocks order by UsersCount desc";
            JsonResult res = SqlQueryTable(query);
            return res;
        }

        [HttpGet("Balance/{id}")]
        public async Task<ActionResult<IEnumerable<Balance>>> GetBalance(int id)
        {
            int IdUser = id;
            var  balances = await dbContext.Balances.Where(x => x.IdUser == id).ToListAsync();
            if (balances == null)
                return NotFound();
            return new ObjectResult(balances);

        }

        [HttpGet("BalanceByDay/{id}")]
        public JsonResult GetBalanceByday(int id)
        {
            //incorrect! its gets sum while should logivally get 
            string query = "select Id_user, Sum(CurrentBalance) as balance, [date] from balance " +
                "where Id_user = "+id+"  group by Id_user, [date]";
            JsonResult res = SqlQueryTable(query);
            return res;
        }


        [HttpGet("Recomendations/{id}")]
        public JsonResult GetRecomend(int id)
        {
            string recomendationQuery = "select top 5 Id_stocks,[Name] ,Count(DISTINCT Id_user) "+
            "as UsersCount from user_stocks, stocks where stocks.Id = user_stocks.Id_stocks and " +
            "Id_stocks in (select DISTINCT Id_stocks from user_stocks where Id_user != "+ id+
            " and Id_stocks not in (Select Distinct Id_stocks from user_stocks where Id_user = "+  id+"))"+
            "Group by Id_stocks, [Name] "+
            "union select top 1 stocks.Id as Id_stocks , [Name], 0 as UsersCount from stocks "+
            "where Id  not in (select Distinct Id_stocks as Id from user_stocks) "+
            " order by UsersCount desc";
            JsonResult res = SqlQueryTable(recomendationQuery);
            return res;
        }
        public class Result
        {
            public Result() { }
            public int Id_stock { get; set; }
            public string Name { get; set; }
            public double Amount { get; set; }
            public double Value { get; set; }
        }

        [HttpGet("ValueAndAmountList/{id}")]
        public async Task<ActionResult<IEnumerable<Result>>> GetValueandAmount(int id)
        {
            //user_stocks (bought)
            var userSt = await dbContext.UserStocks.Where(x=> x.IdUser == id)
               .ToListAsync();
            var result = userSt
                .GroupBy(l => l.IdStocks)
                .SelectMany(cl => cl.Select(
                    csLine => new Result
                    {
                        Id_stock = cl.Select(c => c.IdStocks).FirstOrDefault(),
                        Amount = (double)cl.Sum(c => c.Amount),
                        Value = (double)cl.Sum(c => c.Value),
                    })).ToList<Result>();
            List<Result> fresult = new List<Result>();
            CheckForDuplicate(result, fresult);
            //user_solds (sold)
            var userSold = await dbContext.UserSolds.Where(x => x.IdUser == id)
               .ToListAsync();
            var result2 = userSold
                .GroupBy(l => l.IdStocks)
                .SelectMany(cl => cl.Select(
                    csLine => new Result
                    {
                        Id_stock = cl.Select(c => c.IdStocks).FirstOrDefault(),
                        Amount = (double)cl.Sum(c => c.Amount),
                        Value = (double)cl.Sum(c => c.Value),
                    })).ToList<Result>();
            List<Result> fresult2 = new List<Result>();
            CheckForDuplicate(result2, fresult2);
            List<Result> final = new List<Result>();
            SumResults(fresult, fresult2, final);
            SetName(final);

            

            return final;
        }
        private void SetName(List<Result> final)
        {
            for (var i = 0; i < final.Count(); i++)
            {
                int id = final[i].Id_stock;
                var stock = dbContext.Stocks.Where(x => x.Id == id).FirstOrDefault();
                final[i].Name = stock.Name;
            }
        }
        private void SumResults(List<Result> bought, List<Result> sold, List<Result> final)
        {
            Boolean onlyBought;
            for (var i = 0; i < bought.Count(); i++)
            {
                onlyBought = true;
                for (var y = 0; y < sold.Count(); y++)
                {
                    if (bought[i].Id_stock == sold[y].Id_stock)
                    {
                        onlyBought = false;
                        Result r = new Result
                        {
                            Id_stock = bought[i].Id_stock,
                            Amount = bought[i].Amount - sold[y].Amount,
                            Value = sold[y].Value - bought[i].Value,
                        };
                        final.Add(r);
                        break;
                    }
                }
                if (onlyBought)
                {
                    bought[i].Value = -bought[i].Value;
                    final.Add(bought[i]);
                }
                
            }
        }

       
        private void CheckForDuplicate(List<Result> result, List<Result> fresult)
        {
            Boolean duplicate = false;
            for (var i = 0; i < result.Count(); i++)
            {

                duplicate = false;
                for (var y = 0; y < fresult.Count(); y++)
                {
                    if (result[i].Id_stock == fresult[y].Id_stock)
                    {
                        duplicate = true;
                        break;
                    }
                }
                if (!duplicate)
                {
                    fresult.Add(result[i]);
                }
            }
        }
        [HttpGet("PortfolioRisk/{id}")]
        public JsonResult GetPortfoliorisk(int id)
        {
            string query = "select Id_stocks, ((Max(market_value) - AVG(market_value))/AvG(market_value))*100 as risk " +
            " from Stocks_change where Id_stocks in (select distinct Id_stocks "+
            " from user_stocks where Id_user = " +id+" ) group by Id_stocks";
            JsonResult res = SqlQueryTable(query);
            return res;
        }

        [HttpGet("Events")]
        public JsonResult GetEventList()
        {
            JsonResult res = SqlQueryTable(
              "select  distinct([type]) from Stocks_tags where [type] like 'Event%'");
            return res;
        }

    }
}
