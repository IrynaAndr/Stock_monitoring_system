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
    public class NotificationsController : ControllerBase
    {
        MonitoringStocksContext dbContext = new MonitoringStocksContext();
        // GET: api/<NotificationsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<NotificationsController>/5
        //get notifs by user id

        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<Notification>>> Get(int id)
        {
            var usernotifs = await dbContext.Notifications.Where(x => x.IdUser == id).OrderByDescending(x => x.Id).ToListAsync();
            if (usernotifs == null)
                return NotFound();
            
            return new ObjectResult(usernotifs);
        }

        [HttpGet("count/{id}")]
        public string GetCount(int id)
        {
            var usernotifs = dbContext.Notifications.Where(x => x.IdUser == id && x.Checked ==false).Count();

            return JsonConvert.SerializeObject(usernotifs);
           
        }

        // POST api/<NotificationsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<NotificationsController>/5
        [HttpPut("seen/{id}")]
        public async Task<ActionResult<Notification>> PutSeen(int id, Notification notif)
        {
            if (notif == null)
            {
                return BadRequest();
            }
            if (!dbContext.Notifications.Any(x => x.Id == id))
            {
                return NotFound();
            }

            notif.Id = id;
            if (notif.Checked == true)
            {
                notif.Checked = false;
            }
            else { notif.Checked = true; }

            dbContext.Update(notif);
            await dbContext.SaveChangesAsync();
            return Ok(notif);
        }

        // DELETE api/<NotificationsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Notification>> Delete(int id)
        {
            Notification notif = dbContext.Notifications.FirstOrDefault(x => x.Id == id);
            if (notif == null)
            {
                return NotFound();
            }
            dbContext.Notifications.Remove(notif);
            await dbContext.SaveChangesAsync();
            return Ok(notif);
        }
    }
}
