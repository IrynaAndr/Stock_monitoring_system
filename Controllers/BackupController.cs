using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace Stocks_monitoring_system_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupController : ControllerBase
    {
        SqlConnection con = new SqlConnection();
        SqlCommand sqlcmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        DataTable dt = new DataTable();

        string backupDIR = "D:\\diplom\\BackupBD";


        [HttpGet]
        public string СreateBackUp(string st)
        {
            con.ConnectionString = @"Data Source=.;Initial Catalog=MonitoringStocks;Integrated Security=True";


            if (!System.IO.Directory.Exists(backupDIR))
            {
                System.IO.Directory.CreateDirectory(backupDIR);
            }
            try
            {
                con.Open();
                sqlcmd = new SqlCommand("backup database MonitoringStocks to disk='" + backupDIR + "\\" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".Bak'", con);
                sqlcmd.ExecuteNonQuery();
                con.Close();
                return JsonConvert.SerializeObject("Backup was created");
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject("Error Occured During DB backup process !<br>" + ex.ToString());

            }

        }

        [HttpGet("list")]
        public List<string> BackupList()
        {
            DirectoryInfo d = new DirectoryInfo(backupDIR);
            FileInfo[] Files = d.GetFiles("*.Bak");
            string str = "";
            int count = 0;
            List<string> backuplist = new List<string>();

            foreach (FileInfo file in Files)
            {
                backuplist.Add(file.Name);
                count++;
            }
            if (count == 0)
            {
                backuplist.Add("There no backups yet");
            }
            return backuplist;
        }
        [HttpGet("restore/{FileName}")]
        public string Restore(string FileName)
        {
            con.ConnectionString = @"Data Source=.;Initial Catalog=MonitoringStocks;Integrated Security=True";

            //JToken token = JObject.Parse(FileName.ToString());
            //string FileNameParse = (string)token.SelectToken("FileName");

            string location = @"D:\diplom\BackupBD\" + FileName;
            if (!System.IO.Directory.Exists(backupDIR))
            {
                return "wrong file path";
            }
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(con.ConnectionString))
                {
                    sqlCon.Open();
                    string query = " ALTER DATABASE MonitoringStocks SET SINGLE_USER WITH Rollback Immediate";
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd = new SqlCommand(query, sqlCon);
                    sqlcmd.ExecuteNonQuery();

                    SqlCommand sqlcmd2 = new SqlCommand();
                    string query2 = " USE MASTER RESTORE DATABASE MonitoringStocks FROM DISK = '" + location + "' WITH REPLACE";
                    sqlcmd2 = new SqlCommand(query2, sqlCon);
                    sqlcmd2.ExecuteNonQuery();

                    string query3 = " ALTER DATABASE MonitoringStocks SET MULTI_USER WITH Rollback Immediate";
                    SqlCommand sqlcmd3 = new SqlCommand();
                    sqlcmd3 = new SqlCommand(query3, sqlCon);
                    sqlcmd3.ExecuteNonQuery();
                    sqlCon.Close();

                }

                return JsonConvert.SerializeObject("Database was restored from - " + FileName);

            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(FileName + "." + location + ". Error Occured During DB restore process !<br>" + ex.ToString()); ;

            }
        }
    }
}
