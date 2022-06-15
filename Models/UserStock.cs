using System;
using System.Collections.Generic;

#nullable disable

namespace Stocks_monitoring_system_backend.Models
{
    public partial class UserStock
    {
        public int Id { get; set; }
        public int IdStocks { get; set; }
        public int IdUser { get; set; }
        public double? Amount { get; set; }
        public double? Fee { get; set; }
        public DateTime? PurchaseDate { get; set; }

        public double? Value { get; set; }

        public virtual Stock IdStocksNavigation { get; set; }
        public virtual User IdUserNavigation { get; set; }
    }
}
