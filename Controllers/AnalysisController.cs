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

using Extreme.DataAnalysis;
using Extreme.Statistics;

namespace Stocks_monitoring_system_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        MonitoringStocksContext dbContext = new MonitoringStocksContext();

        //

        

        [HttpPut("Result")]
        public async  Task<ActionResult<ResultAnalysis>> PutResult(StocksTag st)
        {
            string Event = st.Type;
            ResultAnalysis result = new ResultAnalysis();
            //prepare data for analisys - StockData classes
            List<StockData> data = new List<StockData>();
            //get ids and event choise(tag) related to chosen event
            data = preparedData(Event);
            //get results
            result = analysis(data);

            return result;
        }

        [HttpPut("Data")]
        public async Task<ActionResult<IEnumerable<StockData>>> ShowData(StocksTag st)
        {
            string Event = st.Type;
            List<StockData> data = new List<StockData>();
            data = preparedData(Event);

            return data;
        }

        //
        public class StockData
        {
            public StockData() { }
            public int Id_stock { get; set; }

            public string Name { get; set; }
            public string Event { get; set; }
            public double PriceChange { get; set; }
        }
        //
        public class ResultAnalysis
        {
            public ResultAnalysis() { }
            public string Balanced { get; set; }
            public string Table { get; set; }
            public List<string> Means { get; set; }
            public double Observation { get; set; }
            public double GrandMean { get; set; }
        }

        private ResultAnalysis analysis(List<StockData> data)
        {
            var dataFrame = DataFrame.FromObjects(data);
            // Construct the OneWayAnovaModel object.
            var anova = new OneWayAnovaModel(dataFrame, "PriceChange", "Event");
            // Alternatively, you can use a formula to specify the variables:
            anova = new OneWayAnovaModel(dataFrame, "PriceChange ~ Event");
            anova.Compute();
            ResultAnalysis res = new ResultAnalysis();
            // Verify that the design is balanced:
            if (!anova.IsBalanced)
                res.Balanced = "The design is not balanced.";
            // The AnovaTable property gives us a classic ANOVA table.
            res.Table = anova.AnovaTable.ToString();
            // access the group means of our event groups.
            //get the index to easily iterate through the levels:
            var eventFactor = anova.GetFactor<string>(0);

            List<string> Means = new List<string>();
            foreach (string level in eventFactor) {
                string s = String.Format("Mean for group '{0}': {1:F4}",
                    level, anova.Cells.Get(level).Mean);
                    Means.Add(s);
                    //"': " + anova.Cells.GetValue());
            }
            res.Means = Means;
            //get the summary data for the entire model
            // from the TotalCell property:
            Cell totalSummary = anova.TotalCell;
            res.Observation = totalSummary.Count;
            res.GrandMean =  totalSummary.Mean ;

            return res;
        }
        //System.NullReferenceException: 'Object reference not set to an instance of an object.'

        private List<StockData> preparedData(string Event)
        {
            List<StockData> data = new List<StockData>();

            con.ConnectionString = @"Data Source=.;Initial Catalog=MonitoringStocks;Integrated Security=True";
            DataTable table = new DataTable();
            SqlDataReader myReader;
            string query = "select Stocks_tags.*, [Name] from Stocks_tags, Stocks where Stocks_tags.[type] = '" + 
                Event+ "' and Stocks_tags.Id_stocks = Stocks.Id";
            using (SqlConnection sqlCon = new SqlConnection(con.ConnectionString))
            {
                sqlCon.Open();
                using (SqlCommand sqlcmd = new SqlCommand(query, sqlCon))
                {
                    myReader = sqlcmd.ExecuteReader();
                    table.Load(myReader);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        StockData sd = new StockData();
                        sd.Id_stock = int.Parse(table.Rows[i]["Id_stocks"].ToString());
                        sd.Event = table.Rows[i]["tag"].ToString();
                        sd.Name = table.Rows[i]["Name"].ToString();
                        //get priceChange
                        sd.PriceChange = FindPriceChangeStock(sd.Id_stock);

                        data.Add(sd);
                    }
                   
                    myReader.Close();
                    sqlCon.Close();
                }
            }

            return data;
        }

        SqlConnection con = new SqlConnection();
        private double FindPriceChangeStock(int id)
        {
            double pc = 0;
            double last = 0;
            double previous = 0;
            con.ConnectionString = @"Data Source=.;Initial Catalog=MonitoringStocks;Integrated Security=True";
            DataTable table = new DataTable();
            SqlDataReader myReader;
            string query = "Select top 2 market_value,Stocks.* , [date] from Stocks, Stocks_change where Stocks.Id = Stocks_change.Id_stocks and stocks.Id=" + id + " order by[date] DESC";
            using (SqlConnection sqlCon = new SqlConnection(con.ConnectionString))
            {
                sqlCon.Open();
                using (SqlCommand sqlcmd = new SqlCommand(query, sqlCon))
                {
                    myReader = sqlcmd.ExecuteReader();
                    table.Load(myReader);
                    for (int i =0; i<  table.Rows.Count; i++)
                    {
                        if (i == 1)
                        {
                            last = double.Parse(table.Rows[i]["market_value"].ToString());
                        }
                        if (i == 2)
                        {
                            previous = double.Parse(table.Rows[i]["market_value"].ToString());
                        }
                    }
                    pc = last - previous;
                    myReader.Close();
                    sqlCon.Close();
                }
            }
            return pc;
        }
    }
}
