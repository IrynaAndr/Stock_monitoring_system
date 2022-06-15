using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Stocks_monitoring_system_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stocks_monitoring_system_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginRegisterController : ControllerBase
    {
        //log in
        MonitoringStocksContext dbContext = new MonitoringStocksContext();
        [HttpPost]
        public string Post([FromBody] User value)
        {
            if (dbContext.Users.Any(user => user.Login.Equals(value.Login)))
            {

                User user = dbContext.Users.Where(u => u.Login.Equals(value.Login)).First();
                var Password = value.Password;
                if(user.Password.Length <= 12) // for few old logins without hashed passwords 
                {
                    if (Password.Equals(user.Password)){
                            return JsonConvert.SerializeObject(user);}
                    else { return JsonConvert.SerializeObject("Wrong password"); }
                }
                else if (BCrypt.Net.BCrypt.Verify(value.Password, user.Password)) 
                {
                    Response.Cookies.Append("IdUser", user.Id.ToString());
                    return JsonConvert.SerializeObject(user);

                }
                else
                {
                    return JsonConvert.SerializeObject("Wrong password");
                }
            }
            else
            {
                return JsonConvert.SerializeObject("User not existing in Database");
            }

        }

        //register
        SqlConnection con = new SqlConnection();

        [HttpPost("register")]
        public string Post2([FromBody] User value)
        {
            if (!dbContext.Users.Any(user => user.Login.Equals(value.Login)))
            {
                User user = new User();
                user.Login = value.Login; //assign value from post to user
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(value.Password);
                user.Password = passwordHash;
                user.Email = value.Email;
                user.Name = value.Name;
                user.Type = 1;
                try
                {
                    dbContext.Add(user);
                    dbContext.SaveChanges();

                    con.ConnectionString = @"Data Source=.;Initial Catalog=MonitoringStocks;Integrated Security=True";

                    using (SqlConnection sqlCon = new SqlConnection(con.ConnectionString))
                    {
                        sqlCon.Open();
                        string query = "Select max(Id) from Users";
                        SqlCommand sqlcmd = new SqlCommand();
                        sqlcmd = new SqlCommand(query, sqlCon);
                        int result = (int)sqlcmd.ExecuteScalar();
                        //Response.Cookies.Append("IdUser", result.ToString());
                    }

                    return JsonConvert.SerializeObject("Register successfully");
                }
                catch (Exception ex)
                {
                    return JsonConvert.SerializeObject(ex.Message);
                }
            }
            else
            {
                return JsonConvert.SerializeObject("User with this login already exists in Database");
            }

        }

        [HttpPost("test/{str}")]
        public string Post3(string str)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(str);
            bool verified = BCrypt.Net.BCrypt.Verify(str, passwordHash);
            
            return "hash = "+ passwordHash + ". verified 1 = "+verified;
        }
    }
}
