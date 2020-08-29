using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InstantTutors.Controllers
{
    public class ApproveDeclineController : Controller
    {
        // GET: ApproveDecline
        public ActionResult Index()
        {
            try
            {
                DBManage.DBHandler dbHandler = new DBManage.DBHandler();
                Models.StatusMessageModel StatusMessageModel = new Models.StatusMessageModel();
                if (Request.QueryString["action"] != null && Request.QueryString["id"] != null)
                {
                    switch (Request.QueryString["action"])
                    {
                        case "approve":
                            dbHandler.TranscatData("update Sessions set status=1 where id=" + Request.QueryString["id"] + "");
                            StatusMessageModel.Message = ConfigurationManager.AppSettings["ApprovedMessage"];
                            StatusMessageModel.isApproved = true;
                            return View("StatusMessage", StatusMessageModel);                            
                        case "decline":
                            dbHandler.TranscatData("update Sessions set status=2 where id=" + Request.QueryString["id"] + "");
                            StatusMessageModel.Message = ConfigurationManager.AppSettings["DeclinedMessage"];
                            StatusMessageModel.isApproved = false;
                            return View("StatusMessage", StatusMessageModel);
                    }
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
            return Content("Request Processed Succesfully");
        }
    }
}