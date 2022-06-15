using System;
using System.Collections.Generic;

#nullable disable

namespace Stocks_monitoring_system_backend.Models
{
    public partial class Notification
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }

        public Boolean? Checked { get; set; }

        public virtual User IdUserNavigation { get; set; }
    }
}
