using InstantTutors.Areas.Admin.ViewModels;
using InstantTutors.Services;
using InstantTutors.Services.Admin;
using InstantTutors.Services.Tutor;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace InstantTutors.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IDashboardService _dashboardService;
        private ITutorService _tutorService;

        [InjectionConstructor]
        public DashboardController(IDashboardService dashboardService, ITutorService tutorService)
        {
            _dashboardService = dashboardService;
            _tutorService = tutorService;
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
        
        public async Task<ActionResult> Index()
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            AdminViewModel model = await _dashboardService.GetAdminDashboardAsync();
            model.UserId = _user.Id;
            model.User = _user;
            model.StudentsList = model.StudentsList.Take(5).ToList();
            model.TutorsList = model.TutorsList.Take(5).ToList();
            return View(model);
        }

        public async Task<ActionResult> Tutors()
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            AdminViewModel model = await _dashboardService.GetAdminDashboardAsync();
            model.UserId = _user.Id;
            model.User = _user;
            //model.StudentsList = model.StudentsList.Take(5).ToList();
            model.TutorsList = model.TutorsList.ToList();
            return View("Tutor", model);
        }

        public async Task<ActionResult> Students()
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            AdminViewModel model = await _dashboardService.GetAdminDashboardAsync();
            model.UserId = _user.Id;
            model.User = _user;
            model.StudentsList = model.StudentsList.ToList();
            //model.TutorsList = model.TutorsList.ToList();            
            return View("Student", model);
        }

        public async Task<ActionResult> ActiveDeactiveTutor()
        {
            string userid = Request.QueryString["userid"] ?? "";
            utilityService.ActivateDeactivateUsers(userid);
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            AdminViewModel model = await _dashboardService.GetAdminDashboardAsync();
            model.UserId = _user.Id;
            model.User = _user;
            //model.StudentsList = model.StudentsList.Take(5).ToList();
            model.TutorsList = model.TutorsList.ToList();
            ViewBag.success = "<div class='alert alert-success'><strong>Success!</strong> User status has been changed successfully.</div>";
            return View("Tutor", model);
        }

        public async Task<ActionResult> ActiveDeactivestudent()
        {
            string userid = Request.QueryString["userid"] ?? "";
            utilityService.ActivateDeactivateUsers(userid);
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            AdminViewModel model = await _dashboardService.GetAdminDashboardAsync();
            model.UserId = _user.Id;
            model.User = _user;
            //model.StudentsList = model.StudentsList.Take(5).ToList();
            model.TutorsList = model.TutorsList.ToList();
            ViewBag.success = "<div class='alert alert-success'><strong>Success!</strong> User status has been changed successfully.</div>";
            return View("Student", model);
        }
    }
}