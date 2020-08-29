using InstantTutors.Areas.Student.ViewModels;
using InstantTutors.DBManage;
using InstantTutors.Helpers;
using InstantTutors.Models;
using InstantTutors.Models.ViewModels;
using InstantTutors.Services;
using InstantTutors.Services.Student;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace InstantTutors.Areas.Student.Controllers
{
    [Authorize(Roles = "Student")]
    //[OutputCache(Duration = 360)]
    public class DashboardController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ISessionService _sessionService;
        [InjectionConstructor]
        public DashboardController(ISessionService sessionService)
        {
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



        // GET: Student/Dashboard
        public async Task<ActionResult> Index()
        {
            AccountService accountService = new AccountService();
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            StudentViewModel model = new StudentViewModel()
            {
                UserId = _user.Id,
                User = _user
            };
            model.SessionsList = await _sessionService.GetSessionByUserIdAsync(_user.Id, 5);
            if (model.SessionsList != null && model.SessionsList.Count > 0)
            {
                for (int i = 0; i < model.SessionsList.Count; i++)
                {
                    model.SessionsList[i].TutorName = await accountService.GetFullName(model.SessionsList[i].TutorUserId);
                }
            }
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> MessageCenter()
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            MessageCenterViewModel model = new MessageCenterViewModel();
            model.Tutors = GetTutors(_user.Id);
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
            model.Tutors = GetTutors(_user.Id);
            model.Messages = utilityService.GetMessagesTutor(User.Identity.Name);
            return View("Messages", model);
        }

        private List<SelectListItem> GetTutors(string id = "")
        {
            DBHandler dbHandlerObj = new DBHandler();
            DataTable dt = new DataTable();
            string query = "SELECT  U.ID,";
            query += " U.[FIRSTNAME] + ' ' + U. [LASTNAME] + ' - ' + U.EMAIL AS NAME";
            query += " FROM [DBO].[TUTORS] T";
            query += " INNER JOIN[DBO].[ASPNETUSERS]  U";
            query += " ON T.USERID = U.ID";
            if (id != "")
                query += " where U.id in (select distinct tutoruserid from [sessions] where userid='" + id + "' and status=1)";
            dt = dbHandlerObj.GetData(query);
            List<SelectListItem> tutors = new List<SelectListItem>();
            tutors.Add(new SelectListItem
            {
                Text = "Select Tutor -",
                Value = ""
            });

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                tutors.Add(new SelectListItem
                {
                    Text = dt.Rows[i][1].ToString(),
                    Value = dt.Rows[i][0].ToString()
                });
            }

            return tutors;
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
