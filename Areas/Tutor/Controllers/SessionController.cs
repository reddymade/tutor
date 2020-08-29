using InstantTutors.Areas.Student.ViewModels;
using InstantTutors.Areas.Tutor.ViewModels;
using InstantTutors.DBManage;
using InstantTutors.Helpers;
using InstantTutors.Models;
using InstantTutors.Services;
using InstantTutors.Services.Student;
using InstantTutors.Services.Tutor;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace InstantTutors.Areas.Tutor.Controllers
{
    //[RedirectLoginFilter]
    [Authorize(Roles = "Tutor")]
    //[OutputCache(Duration = 360)]
    public class SessionController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ITuitionService _tuitionService;
        private ITutorService _tutorService;
        private ISessionService _sessionService;
        private IScheduleService _scheduleService;

        [InjectionConstructor]
        public SessionController(ITuitionService tuitionService, ITutorService tutorService, ISessionService sessionService,
            IScheduleService scheduleService)
        {
            _tuitionService = tuitionService;
            _tutorService = tutorService;
            _sessionService = sessionService;
            _scheduleService = scheduleService;
        }

        public SessionController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Tutor/Session
        public async Task<ActionResult> Index()
        {

            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            TutorViewModel model = new TutorViewModel()
            {
                UserId = _user.Id,
                User = _user
            };
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

        public async Task<ActionResult> Availability()
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            TutorViewModel model = await _tutorService.GetTutorAvailabilityAsync(_user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Availability(TutorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _tuitionService.SetAvailabilityAsync(model);
                    ModelState.Clear();
                    ViewBag.success = "<ul><li><p style='color:green'>Successfully Submitted!! Thanks for providing information.</p></li></ul>";

                    var t = Task.Run(() =>
                    {
                        try
                        {
                            var _subject = "Tuition Availability - " + model.User.FirstName + " " + model.User.LastName + " | instanttutors.org";
                            var _body = "<b>Tutor Availability Updation</b><br><br/> "
                                + "Tutor : <b>" + model.User.FirstName + " " + model.User.LastName + "</b><br/>"
                                + "Tutor has updated Tuition Availability Information. <br/>"
                                + "Comment/Concerns: " + model.Concerns + "<br/><br/>"
                                + "<a href='http://instanttutors.org/' target='_blank'>Instant Tutors</a> @ " + DateTime.Now.Year;

                            EmailSender.SendEmail(_subject, _body);
                            SMSSender.SMSSenderAsync("Tutor " + model.User.FirstName + " " + model.User.LastName + " just updated tuition availability time.");
                        }
                        catch { }
                    });

                    return View(model);
                }
                catch (Exception ex)
                {
                    ViewBag.success = "<ul><li><p style='color:red'>ERROR: " + ex.Message + "</p></li></ul>";
                    return View(model);
                }
            }
            else
            {
                var error = ModelState.FirstOrDefault(x => x.Value.Errors.Count > 0).Value.Errors.First().ErrorMessage;
                ViewBag.success = "<ul><li><p style='color:red'>ERROR: " + error + "</p></li></ul>";
            }
            return View(model);
        }

        public async Task<ActionResult> Schedule(int? id)
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            var _session = await _sessionService.GetSessionByIdAsync(id ?? 0);
            _session = _session == null ? new Sessions() : _session;

            TutorSessionViewModel model = new TutorSessionViewModel()
            {
                Id = _session.Id,
                Title = _session.Title,
                Description = _session.Description,
                Concerns = _session.Concerns,
                CommunicationMethod = _session.CommunicationMethod,
                CreatedDate = _session.CreatedDate == null ? DateTime.Now : _session.CreatedDate,
                CreatedBy = _session.CreatedBy,
                UserId = _user.Id,
                User = _user,
                Student = _session.UserId,
                StartDate = _session.StartDate,
                EndDate = _session.EndDate
            };

            var SessionSchedules = new List<TutorSessionScheduleViewModel>();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList().Skip(1))
            {
                SessionSchedules.Add(new TutorSessionScheduleViewModel()
                {
                    //Id
                    UserId = _user.Id,
                    SessionId = _session.Id,
                    Timing = Utils.Common.GetTiming(),
                    Day = day
                });
            }
            if (_session.SessionSchedule != null)
            {
                if (_session.SessionSchedule.Count > 0)
                {
                    foreach (var schedule in SessionSchedules)
                    {
                        foreach (var _time in schedule.Timing)
                        {
                            var availTime = _session.SessionSchedule.FirstOrDefault(x => x.Time == _time.AvailabilityTime && x.Day == schedule.Day);
                            if (availTime != null)
                            {
                                schedule.Id = availTime.Id;
                                _time.Selected = true;
                                schedule.Time = availTime.Time;
                            }
                        }
                    }
                }
            }

            if (Request.QueryString["tid"] != null)
            {
                model.Student = Request.QueryString["tid"]?.ToString();
                model.IsRequestComingFromTutor = true;
            }
            model.SessionSchedules = SessionSchedules;
            model.Students = GetStudents();
            return View("Schedule", model);
        }
        private List<SelectListItem> GetStudents(int id = 0)
        {
            DBHandler dbHandlerObj = new DBHandler();
            DataTable dt = new DataTable();
            string query = "SELECT  U.ID, U.[FIRSTNAME] + ' ' + U. [LASTNAME] + ' - ' + U.EMAIL AS NAME FROM [DBO].[ASPNETUSERS]";
            query += " U where U.id not in (select UserId from Tutors) and U.[FIRSTNAME] + ' ' + U. [LASTNAME] + ' - ' + U.EMAIL is not null";


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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectTiming(TutorSessionViewModel model)
        {
            if (model.StartDate != null && model.EndDate != null)
            {
                if (model.StartDate > model.EndDate)
                {
                    ModelState.AddModelError("EndDate", "End date should be less than start date");
                }
                TimeSpan diffResult = model.EndDate.GetValueOrDefault().Subtract(model.StartDate.GetValueOrDefault());
                if (diffResult.Days > 14)
                {
                    ModelState.AddModelError("EndDate", "End date should be two weeks of duration from start date");
                }
            }
            if (ModelState.IsValid)
            {
                var SessionSchedules = new List<TutorSessionScheduleViewModel>();
                var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
                var _session = await _sessionService.GetSessionByIdAsync(model.Id);
                _session = _session == null ? new Sessions() : _session;
                model.CreatedBy = _session.CreatedBy;
                for (DateTime date = model.StartDate.GetValueOrDefault(); date.Date <= model.EndDate; date = date.AddDays(1))
                {
                    SessionSchedules.Add(new TutorSessionScheduleViewModel()
                    {
                        //Id
                        UserId = _user.Id,
                        SessionId = _session.Id,
                        Timing = Utils.Common.GetTiming(),
                        Day = date.DayOfWeek,
                        SelectedDate = date
                    }); ;
                }
                if (_session.SessionSchedule != null)
                {
                    if (_session.SessionSchedule.Count > 0)
                    {
                        foreach (var schedule in SessionSchedules)
                        {
                            foreach (var _time in schedule.Timing)
                            {
                                var availTime = _session.SessionSchedule.FirstOrDefault(x => x.Time == _time.AvailabilityTime
                                && x.Day == schedule.Day && x.SelectedDate == schedule.SelectedDate);
                                if (availTime != null)
                                {
                                    schedule.Id = availTime.Id;
                                    _time.Selected = true;
                                    schedule.Time = availTime.Time;
                                }
                            }
                        }
                    }
                }
                model.SessionSchedules = SessionSchedules;
                return View("SelectTiming", model);
            }
            model.Students = GetStudents();
            return View("Schedule", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Schedule(TutorSessionViewModel model)
        {
            int insertedID = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    var sessionInfo = string.Empty;
                    int DayTimeChecked = 0;
                    if (model.SessionSchedules != null)
                    {
                        foreach (var schedule in model.SessionSchedules)
                        {
                            foreach (var time in schedule.Timing)
                            {
                                if (time.Selected)
                                {
                                    DayTimeChecked += 1;
                                    sessionInfo += "<b>" + schedule.Day.ToString() + "</b>&nbsp;&nbsp;&nbsp;<b>" + time.Time + "</b>&nbsp;&nbsp;&nbsp;<b><br/>";
                                }
                            }
                        }
                    }
                    if (DayTimeChecked < 2)
                    {
                        ViewBag.success = "<ul><li><p style='color:red'>Validation ERROR: Please choose atleast 2+ Days/Times that you are available.</p></li></ul>";
                        model.Students = GetStudents();
                        return View("Schedule", model);
                    }

                    var message = "New Session has been scheduled.";
                    if (model.Id > 0)
                    {
                        (bool success, TutorSessionViewModel data) = await _scheduleService.UpdateScheduleAsync(model);
                        insertedID = data.Id;
                        if (success == false)
                        {
                            ViewBag.success = "<ul><li><p style='color:red'>ERROR: Session Doesn't Exist.</p></li></ul>";
                            model.Students = GetStudents();
                            return View("Schedule", model);
                        }
                        message = "A scheduled session has been updated.";
                    }
                    else
                    {
                        (bool success, TutorSessionViewModel data) = await _scheduleService.CreateScheduleAsync(model);
                        insertedID = data.Id;
                    }

                    var _subject = "Tutor Session Schedule Info - " + model.User.FirstName + " " + model.User.LastName + " | instanttutors.org";
                    var _body = "<p>Greetings,</p>";
                    _body += "<p>" + model.Title.ToUpper() + "</p>";
                    _body += "<p>Tutor: " + model.User.FirstName + " " + model.User.LastName + " is requesting a session for the following days</p>";
                    _body += "<p><span style='text - decoration: underline;'>AVAILABLE ON :</span></p>";
                    _body += "<p>" + sessionInfo + "</p>";
                    _body += "<p>if you would like to approve it click the below link.</p>";
                    _body += "<p>";
                    _body += "<a href='" + ConfigurationManager.AppSettings["SiteURL"] + "?action=approve&code=SGEnYsdguwwhXuGurbIVrr2UviISKcPAFJwJm9e93MsygYBk60sxU1qH%&id=" + insertedID + "'>" +
                    "'" + ConfigurationManager.AppSettings["SiteURL"] + "?action=approve&code=SGEnYsdguwwhXuGurbIVrr2UviISKcPAFJwJm9e93MsygYBk60sxU1qH%&id=" + insertedID + "'</p>";
                    _body += "<p>if you would like to decline the tutor request, you can click the below link :</p>";
                    _body += "<a href='" + ConfigurationManager.AppSettings["SiteURL"] + "?action=decline&code=SGEnYsdguwwhXuGurbIVrr2UviISKcPAFJwJm9e93MsygYBk60sxU1qH%&id=" + insertedID + "'>" +
                    "'" + ConfigurationManager.AppSettings["SiteURL"] + "?action=decline&code=SGEnYsdguwwhXuGurbIVrr2UviISKcPAFJwJm9e93MsygYBk60sxU1qH%&id=" + insertedID + "'</p>";
                    _body += "<p>--</p>";
                    _body += "<p>Regards from&nbsp;<a href='http://instanttutors.org/' target='_blank' rel='noopener'>Intant Tutors Team</a> &nbsp;</p>";
                    _body += "<p>&nbsp;</p>";
                    _body += "<p>Mark&nbsp;<a href='http://instanttutors.org/Inappropriate?action=Inappropriate&Fromid=" + model.UserId + "&Forid=" + model.Student + "' target='_blank' rel='noopener'>Inappropriate</a>&nbsp; , report to administrator</p>";

                    var _userInfo = UserManager.FindByIdAsync(model.Student);
                    string emailTo = utilityService.GetEmailAddress(model.Student);

                    await EmailSender.SendEmailAsync(_subject, _body, emailTo);
                    //await SMSSender.SMSSenderAsync(message + " Tutor name is " + model.User.FirstName + " " + model.User.LastName);

                    ModelState.Clear();
                    //ViewBag.success = "<div class='alert alert-success'><strong>Success!</strong> Session info successfully submitted.</div>";
                    //model.Students = GetStudents();


                    ViewBag.success = "<div class='alert alert-success'><strong>Success!</strong> " + message + "</div>";
                    //model.Tutors = GetTutors();
                    System.Threading.Thread.Sleep(1000);
                    var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
                    TutorViewModel _tutorViewModel = new TutorViewModel()
                    {
                        UserId = _user.Id,
                        User = _user
                    };
                    _tutorViewModel.SessionsList = await _sessionService.GetSessionByTutorIdAsync(_user.Id);
                    return View("Index", _tutorViewModel);
                }
                catch (Exception ex)
                {
                    ViewBag.success = "<div class='alert alert-danger'><strong>ERROR!</strong> " + ex.Message + ".</div>";
                    model.Students = GetStudents();
                    return View("Schedule", model);
                }
            }
            model.Students = GetStudents();
            return View("Schedule", model);
        }

        public async Task<ActionResult> DeleteSession(int? id)
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            if (id != null && id > 0)
            {
                DBManage.DBHandler dbHandler = new DBManage.DBHandler();
                dbHandler.TranscatData("delete Sessions where id=" + id + "");
            }
            TutorViewModel model = new TutorViewModel()
            {
                UserId = _user.Id,
                User = _user
            };
            model.SessionsList = await _sessionService.GetSessionByTutorIdAsync(_user.Id);
            return View("Index", model);
        }


        public async Task<ActionResult> ApproveSession(int? id)
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            if (id != null && id > 0)
            {
                DBManage.DBHandler dbHandler = new DBManage.DBHandler();
                dbHandler.TranscatData("update Sessions set status=1 where id=" + id + "");
            }
            TutorViewModel model = new TutorViewModel()
            {
                UserId = _user.Id,
                User = _user
            };
            model.SessionsList = await _sessionService.GetSessionByTutorIdAsync(_user.Id);
            return View("Index", model);

        }

        public async Task<ActionResult> DeclineSession(int? id)
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            if (id != null && id > 0)
            {
                DBManage.DBHandler dbHandler = new DBManage.DBHandler();
                dbHandler.TranscatData("update Sessions set status=2 where id=" + id + "");
            }
            TutorViewModel model = new TutorViewModel()
            {
                UserId = _user.Id,
                User = _user
            };
            model.SessionsList = await _sessionService.GetSessionByTutorIdAsync(_user.Id);
            return View("Index", model);
        }
    }
}
