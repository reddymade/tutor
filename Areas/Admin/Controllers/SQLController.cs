using Dapper;
using InstantTutors.Areas.Admin.ViewModels;
using InstantTutors.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace InstantTutors.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SQLController : Controller
    {
        [Dependency]
        public ApplicationDbContext _dbContext { get; set; }

        public async Task<ActionResult> Index()
        {
            SQLViewModel model = new SQLViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> Index(string query)
        {            
            if (string.IsNullOrEmpty(query))
                return Json(null, JsonRequestBehavior.AllowGet);

            using (var database = _dbContext)
            {
                //var results = database.Query(sql, parameters);
                string connectionString = ConfigurationManager.ConnectionStrings["ITutorsConnection"].ConnectionString;

                using (SqlConnection Connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        await Connection.OpenAsync();
                        //var recordMultiple = await Connection.QueryMultipleAsync(query);
                        var record = await Connection.QueryAsync(query);
                        if (record != null)
                        {
                            var obj = record.ToList();
                            return Json(record.ToList(), JsonRequestBehavior.AllowGet);
                            //var data = recordMultiple.Read<dynamic>().ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        Connection.Close();
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}