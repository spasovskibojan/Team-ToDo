using IT_Proekt_Proba_Teams.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IT_Proekt_Proba_Teams.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AdminController()
        {
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                _userManager.Dispose();
                _roleManager.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Admin
        public ActionResult Index()
        {
            var model = new AdminDashboardViewModel();

            model.TotalUsers = _userManager.Users.Count();
            model.TotalTeams = db.Teams.Count();
            model.TotalTasks = db.TaskModels.Count();
            model.TotalCompletedTasks = db.TaskModels.Count(t => t.IsCompleted);
            model.TotalActiveTasks = db.TaskModels.Count(t => !t.IsCompleted);

            var teamLeaderRole = _roleManager.FindByName("TeamLeader");
            var employeeRole = _roleManager.FindByName("Employee");

            if (teamLeaderRole != null)
            {
                model.TotalTeamLeaders = _userManager.Users.Count(u => u.Roles.Any(r => r.RoleId == teamLeaderRole.Id));
            }
            if (employeeRole != null)
            {
                model.TotalEmployees = _userManager.Users.Count(u => u.Roles.Any(r => r.RoleId == employeeRole.Id));
            }

            // Дополнителни листи за преглед (ако ги додадеш во ViewModel)
            // model.LatestRegisteredUsers = _userManager.Users.OrderByDescending(u => u.Id).Take(5).ToList();
            // model.LatestTeams = db.Teams.OrderByDescending(t => t.Id).Take(5).ToList();
            // model.LatestTasks = db.TaskModels.OrderByDescending(t => t.Id).Take(5).ToList();

            return View(model);
        }

        // Можеш да додадеш и други акции тука, на пр. за управување со корисници/улоги.
        // public ActionResult ManageUsers() { ... }
        // public ActionResult ManageRoles() { ... }
    }
}