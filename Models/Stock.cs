using System;
using System.Collections.Generic;

#nullable disable

namespace Stocks_monitoring_system_backend.Models
{
    public partial class Stock
    {
        public Stock()
        {
            StocksChanges = new HashSet<StocksChange>();
            StocksTags = new HashSet<StocksTag>();
            UserSolds = new HashSet<UserSold>();
            UserStocks = new HashSet<UserStock>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Symbol { get; set; }
        public string MarketScope { get; set; }
        public string Type { get; set; }
        public string Info { get; set; }

        public double? NetIncome { get; set; }
        public double? WeightedAverage { get; set;  }

        public string Behavior { get; set; }

        public virtual ICollection<StocksChange> StocksChanges { get; set; }
        public virtual ICollection<StocksTag> StocksTags { get; set; }
        public virtual ICollection<UserSold> UserSolds { get; set; }
        public virtual ICollection<UserStock> UserStocks { get; set; }

        public double CalculateExpectedReturn(double marketValue)
        {
            double res = 0;
            res = (double)(this.NetIncome/ (marketValue * this.WeightedAverage));
            return res;
        }
    }
}
