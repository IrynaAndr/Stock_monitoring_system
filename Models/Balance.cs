using System;
using System.Collections.Generic;

#nullable disable

namespace Stocks_monitoring_system_backend.Models
{
    public partial class Balance
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public double CurrentBalance { get; set; }
        public DateTime Date { get; set; }
        public string Msg { get; set; }

        public virtual User IdUserNavigation { get; set; }
    }
}
