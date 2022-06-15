using System;
using System.Collections.Generic;

#nullable disable

namespace Stocks_monitoring_system_backend.Models
{
    public partial class StocksChange
    {
        public int Id { get; set; }
        public int IdStocks { get; set; }
        public double Weight { get; set; }
        public double MarketValue { get; set; }
        public double? StandardDeviation { get; set; }

        public DateTime? Date { get; set; }

        public virtual Stock IdStocksNavigation { get; set; }
    }
}
