using InstantTutors.Areas.Tutor.ViewModels;
using InstantTutors.DBManage;
using InstantTutors.Helpers;
using InstantTutors.Models;
using InstantTutors.Models.ViewModels;
using InstantTutors.Services;
using InstantTutors.Services.Student;
using InstantTutors.Services.Tutor;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace InstantTutors.Areas.Tutor.Controllers
{    
    [Authorize(Roles = "Tutor")]    
    public class DashboardController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ITuitionService _tuitionService;
        private ITutorService _tutorService;
        private ISessionService _sessionService;

        [InjectionConstructor]
        public DashboardController(ITuitionService tuitionService, ITutorService tutorService, ISessionService sessionService)
        {
            _tuitionService = tuitionService;
            _tutorService = tutorService;
            _sessionService = sessionService;
        }

        public DashboardController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Tutor/Dashboard
        public async Task<ActionResult> Index()
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            TutorViewModel model = await _tutorService.GetTutorAvailabilityAsync(_user);
            model.SessionsList = await _sessionService.GetSessionByTutorIdAsync(_user.Id);
            AccountService accountService = new AccountService();
            if (model.SessionsList != null && model.SessionsList.Count > 0)
            {
                for (int i = 0; i < model.SessionsList.Count; i++)
                {
                    model.SessionsList[i].TutorName = await accountService.GetFullName(model.SessionsList[i].UserId);
                }
            }
            return View(model);
        }

        private List<SelectListItem> GetStudents(string id = "")
        {
            DBHandler dbHandlerObj = new DBHandler();
            DataTable dt = new DataTable();
            string query = "SELECT  U.ID, U.[FIRSTNAME] + ' ' + U. [LASTNAME] + ' - ' + U.EMAIL AS NAME FROM [DBO].[ASPNETUSERS]";
            query += " U where U.id not in (select UserId from Tutors) and U.[FIRSTNAME] + ' ' + U. [LASTNAME] + ' - ' + U.EMAIL is not null";
            query += " and  U.id in (select UserId from [dbo].[Sessions] where tutoruserid= '"+ id + "' and status =1)";

            dt = dbHandlerObj.GetData(query);
            List<SelectListItem> students = new List<SelectListItem>();
            students.Add(new SelectListItem
            {
                Text = "Select Student -",
                Value = ""
            });

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                students.Add(new SelectListItem
                {
                    Text = dt.Rows[i][1].ToString(),
                    Value = dt.Rows[i][0].ToString()
                });
            }

            return students;
        }
        public async Task<ActionResult> MessageCenter()
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            MessageCenterViewModel model = new MessageCenterViewModel();
            model.Students = GetStudents(_user.Id);
            model.Messages = utilityService.GetMessagesTutor(User.Identity.Name);
            return View("Messages", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MessageCenter(MessageCenterViewModel model)
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            if (ModelState.IsValid)
            {

                await utilityService.SendMessageTutor(_user.Id, model.Tutor, model.MessageBody, "Message Subject");
                ViewBag.success = "<div class='alert alert-success'><strong>Success!</strong> Message sent successfully</div>";
            }
            model.Students = GetStudents(_user.Id);
            model.Messages = utilityService.GetMessagesTutor(User.Identity.Name);
            return View("Messages", model);
        }

        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Contact(ContactUsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                model.Mobile = string.IsNullOrEmpty(model.Mobile) ? " - " : model.Mobile;
                var _subject = "Contact mail from " + model.FirstName + " " + model.LastName + " | instanttutors.org";
                var _body = "Mail from " + model.FirstName + " " + model.LastName + ",<br/><br/>"
                            + "<b>Email Address:</b> " + model.Email + "<br/>"
                            + "<b>Mobile:</b> " + model.Mobile + "<br/>"
                            + "<b>Comment:</b> " + model.Comment + "<br/><br/>"
                            + "<a href='http://instanttutors.org/' target='_blank'>Instant Tutors</a> @ " + DateTime.Now.Year;

                await EmailSender.SendEmailAsync(_subject, _body);
                ModelState.Clear();
                ViewBag.success = "<ul><li><p style='color:green'>Email has been sent. Thanks for contacting us.</p></li></ul>";
            }
            catch (Exception ex)
            {
                ViewBag.success = "<ul><li><p style='color:red'>ERROR: " + ex.Message + "</p><input type='hidden' value='" + ex + "'/></li></ul>";
            }
            return View();
        }


    }
}
