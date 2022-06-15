using System;
using System.Collections.Generic;

#nullable disable

namespace Stocks_monitoring_system_backend.Models
{
    public partial class StocksTag
    {
        public int Id { get; set; }
        public int IdStocks { get; set; }
        public string Tag { get; set; }

        public string? Type { get; set; }

        public double? Value { get; set; }

        public virtual Stock IdStocksNavigation { get; set; }
    }
}
