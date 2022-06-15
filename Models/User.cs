using System;
using System.Collections.Generic;

#nullable disable

namespace Stocks_monitoring_system_backend.Models
{
    public partial class User
    {
        public User()
        {
            Balances = new HashSet<Balance>();
            Notifications = new HashSet<Notification>();
            UserSolds = new HashSet<UserSold>();
            UserStocks = new HashSet<UserStock>();
        }

        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }

        public virtual ICollection<Balance> Balances { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<UserSold> UserSolds { get; set; }
        public virtual ICollection<UserStock> UserStocks { get; set; }
    }
}
