﻿using InstantTutors.Areas.Admin.ViewModels;
using InstantTutors.Services.Admin;
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
    public class UserController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IDashboardService _dashboardService;

        [InjectionConstructor]
        public UserController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        public async Task<ActionResult> Tutors()
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            AdminViewModel model = await _dashboardService.GetAdminDashboardAsync();
            model.UserId = _user.Id;
            model.User = _user;

            return View(model);
        }
    }
}