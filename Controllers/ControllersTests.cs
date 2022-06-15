using Stocks_monitoring_system_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Stocks_monitoring_system_backend.Controllers
{
    
    public class ControllersTests
    {

        MonitoringStocksContext dbContext = new MonitoringStocksContext();

        [Fact]
        public void CalculateExpectedReturn_SimpleValue_shouldCalculate()
        {
            //arrange
            Stock s = new Stock();
            s.WeightedAverage = 200;
            s.NetIncome = 10000;
            double MarketValue = 5;
            double expected = 10;//10000/(200 *5)
            //act
            double actual = s.CalculateExpectedReturn(MarketValue);
            //assert
            Assert.Equal(expected, actual);
        }
    }
}
