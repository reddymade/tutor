using InstantTutors.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InstantTutors.Controllers
{
    public class InappropriateController : Controller
    {
        // GET: Inappropriate
        public async Task<ActionResult> Index()
        {
            try
            {
                DBManage.DBHandler dbHandler = new DBManage.DBHandler();
                Models.StatusMessageModel StatusMessageModel = new Models.StatusMessageModel();
                if (Request.QueryString["action"] != null && Request.QueryString["Fromid"] != null & Request.QueryString["Forid"] != null)
                {
                    StatusMessageModel.Message = ConfigurationManager.AppSettings["InappropriateMessage"];
                    StatusMessageModel.isApproved = true;
                    string emailTo = ConfigurationManager.AppSettings["AdminEmail"];
                    await EmailSender.SendEmailAsync("Inappropriate Complain Message", "Inappropriate Complain Message", emailTo);
                    return View("StatusMessage", StatusMessageModel);
                }
                else
                {
                    return Content("Error Occoured. Please contact system administrator");
                }
            }
            catch
            {
                return Content("Error Occoured. Please contact system administrator");
            }
        }
    }
}