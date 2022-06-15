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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Stocks_monitoring_system_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        MonitoringStocksContext dbContext = new MonitoringStocksContext();
        // GET: api/<UserController>
        [HttpGet]
        public async Task<ActionResult<User>> Get()
        {
            var user = await dbContext.Users.ToListAsync();
            if (user == null)

                return NotFound();
            return new ObjectResult(user);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            User user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound();
            Response.Cookies.Append("IdUser", user.Id.ToString());
            return new ObjectResult(user);
        }

        // POST api/<UserController>
        [HttpPost]
        public string Post([FromBody] User value)
        {
            User user = new User();
            user.Login = value.Login;
            user.Password = value.Password;
            user.Email = value.Email;
            user.Name = value.Name;
            user.Type = value.Type;

            try
            {
                dbContext.Add(user);
                dbContext.SaveChanges();
                return JsonConvert.SerializeObject("New user was created");
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Put(int id, User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (!dbContext.Users.Any(x => x.Id == id))
            {
                return NotFound();
            }

            user.Id = id;

            dbContext.Update(user);
            await dbContext.SaveChangesAsync();
            return Ok(user);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            User user = dbContext.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();
            return Ok(user);
        }


        
    }
}
