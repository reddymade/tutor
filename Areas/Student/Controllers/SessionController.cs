using InstantTutors.Areas.Student.ViewModels;
using InstantTutors.DBManage;
using InstantTutors.Helpers;
using InstantTutors.Models;
using InstantTutors.Services;
using InstantTutors.Services.Student;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace InstantTutors.Areas.Student.Controllers
{

    //[OutputCache(Duration = 360)]
    public class SessionController : Controller
    {
        public ApplicationDbContext _dbContext { get; set; }
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ISessionService _sessionService;
        private IScheduleService _scheduleService;


        [InjectionConstructor]
        public SessionController(ISessionService sessionService, IScheduleService scheduleService)
        {
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

        // GET: Student/Session
        public async Task<ActionResult> Index()
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            AccountService accountService = new AccountService();
            StudentViewModel model = new StudentViewModel()
            {
                UserId = _user.Id,
                User = _user
            };
            model.SessionsList = await _sessionService.GetSessionByUserIdAsync(_user.Id);

            if (model.SessionsList != null && model.SessionsList.Count > 0)
            {
                for (int i = 0; i < model.SessionsList.Count; i++)
                {
                    model.SessionsList[i].TutorName = await accountService.GetFullName(model.SessionsList[i].TutorUserId);
                }
            }

            return View(model);
        }
        public async Task<ActionResult> NewSchedule(int? id)
        {
            string tutorHashCode = string.Empty;
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            var _session = await _sessionService.GetSessionByIdAsync(id ?? 0);
            _session = _session == null ? new Sessions() : _session;

            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(100);

            SessionViewModel model = new SessionViewModel()
            {
                Id = _session.Id,
                Title = _session.Title,
                Description = _session.Description,
                Concerns = _session.Concerns,
                CommunicationMethod = _session.CommunicationMethod,
                CreatedDate = _session.CreatedDate == null ? DateTime.Now : _session.CreatedDate,
                UserId = _user.Id,
                User = _user
            };

            var SessionSchedules = new List<SessionScheduleViewModel>();

            //if((endDate - startDate).TotalDays > 7)
            //{

            //}

            //for (DateTime date = startDate; date.Date <= endDate.Date; date = date.AddDays(1))
            //{
            //    SessionSchedules.Add(new SessionScheduleViewModel()
            //    {
            //        //Id
            //        UserId = _user.Id,
            //        SessionId = _session.Id,
            //        Timing = Utils.Common.GetTiming(),
            //        Day = date.DayOfWeek
            //    });
            //}

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList().Skip(1))
            {
                SessionSchedules.Add(new SessionScheduleViewModel()
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
            model.SessionSchedules = SessionSchedules;

            if (Request.QueryString["tid"] != null)
            {
                tutorHashCode = Request.QueryString["tid"]?.ToString();
                model.IsRequestComingFromTutor = true;
            }

            model.Tutor = tutorHashCode;
            model.Tutors = GetTutors();
            return View(model);
        }

        private List<SelectListItem> GetTutors(int id = 0)
        {
            DBHandler dbHandlerObj = new DBHandler();
            DataTable dt = new DataTable();
            string query = "SELECT  U.ID,";
            query += " U.[FIRSTNAME] + ' ' + U. [LASTNAME] + ' - ' + U.EMAIL AS NAME";
            query += " FROM [DBO].[TUTORS] T";
            query += " INNER JOIN[DBO].[ASPNETUSERS]  U";
            query += " ON T.USERID = U.ID";
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

        public async Task<ActionResult> DeleteSession(int? id)
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            if (id != null && id > 0)
            {
                DBManage.DBHandler dbHandler = new DBManage.DBHandler();
                dbHandler.TranscatData("delete Sessions where id=" + id + "");
            }
            StudentViewModel model = new StudentViewModel()
            {
                UserId = _user.Id,
                User = _user
            };
            model.SessionsList = await _sessionService.GetSessionByUserIdAsync(_user.Id);
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
            StudentViewModel model = new StudentViewModel()
            {
                UserId = _user.Id,
                User = _user
            };
            model.SessionsList = await _sessionService.GetSessionByUserIdAsync(_user.Id);
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
            StudentViewModel model = new StudentViewModel()
            {
                UserId = _user.Id,
                User = _user
            };
            model.SessionsList = await _sessionService.GetSessionByUserIdAsync(_user.Id);
            return View("Index", model);
        }

        public async Task<ActionResult> Schedule(int? id)
        {
            HttpContext.Session["ScheduleSession"] = null;
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            var _session = await _sessionService.GetSessionByIdAsync(id ?? 0);
            _session = _session == null ? new Sessions() : _session;

            SessionViewModel model = new SessionViewModel()
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
                Tutor = _session.TutorUserId,
                StartDate = _session.StartDate,
                EndDate = _session.EndDate
            };

            var SessionSchedules = new List<SessionScheduleViewModel>();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList().Skip(1))
            {
                SessionSchedules.Add(new SessionScheduleViewModel()
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
            model.SessionSchedules = SessionSchedules;
            model.Tutors = GetTutors();
            return View("NewSchedule", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectTiming(SessionViewModel model)
        {
            if (model.StartDate != null && model.EndDate != null)
            {
                if (model.StartDate > model.EndDate)
                {
                    ModelState.AddModelError("EndDate", "End date should be less than start date");
                }
                TimeSpan diffResult = model.EndDate.GetValueOrDefault().Subtract(model.StartDate.GetValueOrDefault());
                if(diffResult.Days>14)
                {
                    ModelState.AddModelError("EndDate", "End date should be two weeks of duration from start date");
                }
            }

            if (ModelState.IsValid)
            {
                var SessionSchedules = new List<SessionScheduleViewModel>();
                var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
                var _session = await _sessionService.GetSessionByIdAsync(model.Id);
                _session = _session == null ? new Sessions() : _session;
                model.CreatedBy = _session.CreatedBy;
                for (DateTime date = model.StartDate.GetValueOrDefault(); date.Date <= model.EndDate; date = date.AddDays(1))
                {
                    SessionSchedules.Add(new SessionScheduleViewModel()
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
            model.Tutors = GetTutors();
            HttpContext.Session["ScheduleSession"] = model;

            return View("NewSchedule", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Schedule(SessionViewModel model)
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
                                    sessionInfo += "<b>" + schedule.Day.ToString() + "</b>&nbsp;&nbsp;&nbsp;<b>" + time.Time + "</b>&nbsp;&nbsp;&nbsp;<b>" + model.CommunicationMethod + "</b>&nbsp;&nbsp;&nbsp;<b>(" + model.Title + ")</b><br/><br/>";
                                }
                            }
                        }
                    }
                    if (DayTimeChecked < 2)
                    {
                        ViewBag.success = "<ul><li><p style='color:red'>Validation ERROR: Please choose atleast 2+ Days/Times that you are available.</p></li></ul>";
                        model.Tutors = GetTutors();
                        return View("NewSchedule", model);
                    }

                    var message = "New Session has been scheduled.";
                    if (model.Id > 0)
                    {
                        (bool success, SessionViewModel data) = await _scheduleService.UpdateScheduleAsync(model);
                        insertedID = data.Id;
                        if (success == false)
                        {
                            ViewBag.success = "<ul><li><p style='color:red'>ERROR: Session Doesn't Exist.</p></li></ul>";
                            model.Tutors = GetTutors();
                            return View("NewSchedule", model);
                        }
                        message = "A scheduled session has been updated.";
                    }
                    else
                    {
                        (bool success, SessionViewModel data) = await _scheduleService.CreateScheduleAsync(model);
                        insertedID = data.Id;
                    }

                    //var t = Task.Run(() =>
                    //{
                    //    try
                    //    {
                            
                    //    }
                    //    catch { }
                    //});

                    var _subject = "Student Session Schedule Info - " + model.User.FirstName + " " + model.User.LastName + " | instanttutors.org";
                    var _body = "<a class='navbar-brand' href='/'><img src='http://www.instanttutors.org/Content/images/logo.png' style='width:240px;' /></a>";
                    _body += "<p>Greetings,</p>";
                    _body += "<p>" + model.Title.ToUpper() + "</p>";
                    _body += "<p>Student: " + model.User.FirstName + " " + model.User.LastName + " is requesting a session for the following days</p>";
                    _body += "<p><span style='text - decoration: underline;'>AVAILABLE ON :</span></p>";
                    _body += "<p>" + sessionInfo + "</p>";
                    _body += "<p>if you would like to approve it click the below link.</p>";
                    _body += "<p>";
                    _body += "<a href='" + ConfigurationManager.AppSettings["SiteURL"] + "?action=approve&code=SGEnYsdguwwhXuGurbIVrr2UviISKcPAFJwJm9e93MsygYBk60sxU1qH%&id=" + insertedID + "'>" +
                    "'" + ConfigurationManager.AppSettings["SiteURL"] + "?action=approve&code=SGEnYsdguwwhXuGurbIVrr2UviISKcPAFJwJm9e93MsygYBk60sxU1qH%&id=" + insertedID + "'</p>";
                    _body += "<p>if you would like to decline the tutor request, you can click the below link :</p>";
                    _body += "<a href='" + ConfigurationManager.AppSettings["SiteURL"] + "?action=decline&code=SGEnYsdguwwhXuGurbIVrr2UviISKcPAFJwJm9e93MsygYBk60sxU1qH%&id=" + insertedID + "'>" +
                    "" + ConfigurationManager.AppSettings["SiteURL"] + "?action=decline&code=SGEnYsdguwwhXuGurbIVrr2UviISKcPAFJwJm9e93MsygYBk60sxU1qH%&id=" + insertedID + "</p>";
                    _body += "<p>--</p>";
                    _body += "<p>Regards from&nbsp;<a href='http://instanttutors.org/' target='_blank' rel='noopener'>Intant Tutors Team</a> &nbsp;</p>";
                    _body += "<p>&nbsp;</p>";
                    _body += "<p>Mark&nbsp;<a href='http://instanttutors.org/Inappropriate?action=Inappropriate&Fromid="+ model.UserId + "&Forid="+ model.Tutor + "' target='_blank' rel='noopener'>Inappropriate</a>&nbsp; , report to administrator</p>";


                    string emailTo = utilityService.GetEmailAddress(model.Tutor);
                    await EmailSender.SendEmailAsync(_subject, _body, emailTo);
                   // SMSSender.SMSSenderAsync(message + " Student name is " + model.User.FirstName + " " + model.User.LastName);

                    ModelState.Clear();
                    ViewBag.success = "<div class='alert alert-success'><strong>Success!</strong> " + message + "</div>";
                    //model.Tutors = GetTutors();
                    var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
                    StudentViewModel _studentViewModel = new StudentViewModel()
                    {
                        UserId = _user.Id,
                        User = _user
                    };
                    _studentViewModel.SessionsList = await _sessionService.GetSessionByUserIdAsync(_user.Id);
                    return View("Index", _studentViewModel);
                }
                catch (Exception ex)
                {
                    ViewBag.success = "<div class='alert alert-danger'><strong>ERROR!</strong> " + ex.Message + ".</div>";
                    model.Tutors = GetTutors();
                    return View("NewSchedule", model);
                }
            }
            model.Tutors = GetTutors();
            return View("NewSchedule", model);
        }
        private string GetEmailAddress(string id)
        {
            throw new NotImplementedException();
        }
    }
}
