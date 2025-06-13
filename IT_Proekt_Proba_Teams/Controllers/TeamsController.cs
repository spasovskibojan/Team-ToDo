using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IT_Proekt_Proba_Teams.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace IT_Proekt_Proba_Teams.Controllers
{
    [Authorize]
    public class TeamsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public TeamsController()
        {
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }
        // GET: Teams
        public ActionResult Index()
        {
            IQueryable<Team> teams = db.Teams.Include(t => t.TeamLeader);
            if (User.IsInRole("Administrator"))
            {
            }
            else
            {
                string currentUserId = User.Identity.GetUserId();
                var currentUser = _userManager.FindById(currentUserId);

                if (currentUser != null && currentUser.TeamId.HasValue)
                {
                    teams = teams.Where(t => t.Id == currentUser.TeamId.Value);
                }
                else
                {
                    teams = teams.Where(t => false);
                }
            }
            string UserId = User.Identity.GetUserId();
            var UserVar = _userManager.FindById(UserId);
            if (UserVar.TeamId == null)
            {
                ViewBag.ShowCreate = "Yes";
            }
            return View(teams.ToList());
        }

        // GET: Teams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Include(t => t.TeamLeader).Include(t => t.Members).FirstOrDefault(t => t.Id == id); // Вклучи ги и членовите
            if (team == null)
            {
                return HttpNotFound();
            }

            string currentUserId = User.Identity.GetUserId();
            var currentUser = _userManager.FindById(currentUserId);

            if (User.IsInRole("Administrator"))
            {
                // Администраторот има пристап
            }
            else // Тим Лидер и Работник
            {
                if (currentUser == null || !currentUser.TeamId.HasValue || currentUser.TeamId.Value != team.Id)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Немате дозвола да ги видите деталите за овој тим.");
                }
            }
            return View(team);
        }
        [Authorize(Roles = "TeamLeader")]
        public ActionResult InsertNewTeamMember(int teamId)
        {
            string currentUserId = User.Identity.GetUserId();
            Team team = db.Teams.Find(teamId);

            var model = new AddTeamMemberViewModel { TeamId = teamId };
            ViewBag.TeamName = team.Name;

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TeamLeader")]
        public ActionResult InsertNewTeamMember(AddTeamMemberViewModel model)
        {
            string currentUserId = User.Identity.GetUserId();
            Team team = db.Teams.Find(model.TeamId);
            if (ModelState.IsValid)
            {
                var memberToAdd = _userManager.FindByEmail(model.Email);

                // Constraints next 3 ifs
                if (memberToAdd == null)
                {
                    ModelState.AddModelError("Email", "Корисникот со таа е-пошта не е пронајден.");
                    ViewBag.TeamName = team.Name;
                    return View(model);
                }

                if (memberToAdd.TeamId.HasValue)
                {
                    ModelState.AddModelError("Email", "Корисникот веќе е член на друг тим.");
                    ViewBag.TeamName = team.Name;
                    return View(model);
                }
                if (_userManager.IsInRole(memberToAdd.Id, "TeamLeader") || _userManager.IsInRole(memberToAdd.Id, "Administrator"))
                {
                    ModelState.AddModelError("Email", "Не можете да додадете тим лидер или администратор како член на тим.");
                    ViewBag.TeamName = team.Name;
                    return View(model);
                }

                memberToAdd.TeamId = team.Id;
                var result = _userManager.Update(memberToAdd);

                if (result.Succeeded)
                {
                    if (!_userManager.IsInRole(memberToAdd.Id, "Employee"))
                    {
                        _userManager.AddToRole(memberToAdd.Id, "Employee");
                    }
                    return RedirectToAction("Details", new { id = team.Id });
                }
            }

            ViewBag.TeamName = team.Name;
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "TeamLeader")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveMemberFromTeam(int teamId, string memberId)
        {
            string currentUserId = User.Identity.GetUserId();
            Team team = db.Teams.Find(teamId);

            if (team == null || team.TeamLeaderId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Немате дозвола да отстранувате членови од овој тим.");
            }

            var memberToRemove = _userManager.FindById(memberId);

            if (memberToRemove == null || memberToRemove.TeamId != teamId ||
                memberToRemove.Id == team.TeamLeaderId ||
                _userManager.IsInRole(memberToRemove.Id, "Administrator"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Членот не е пронајден во овој тим или не може да биде отстранет.");
            }

            var delegatedTasks = db.TaskModels.Where(t => t.AssignedToUserId == memberId).ToList();

            db.TaskModels.RemoveRange(delegatedTasks); 
            
            memberToRemove.TeamId = null;
            var result = _userManager.Update(memberToRemove);

            if (result.Succeeded)
            {
                if (_userManager.IsInRole(memberToRemove.Id, "Employee"))
                {
                    _userManager.RemoveFromRole(memberToRemove.Id, "Employee");
                }

                db.SaveChanges();
                return RedirectToAction("Details", new { id = teamId });
            }

            ModelState.AddModelError("", "Грешка при отстранување на член од тим.");
            return RedirectToAction("Details", new { id = teamId }); 
        }
        // GET: Teams/Create
        [Authorize(Roles = "Employee")]
        public ActionResult Create()
        {
            string currentUserId = User.Identity.GetUserId();
            var currentUser = _userManager.FindById(currentUserId);
            //_userManager.GetRoles(currentUserId).Any()
            // 1. Корисникот мора да биде Работник (Employee), 2. Корисникот НЕ смее да биде веќе во тим (TeamId == null), 3. Корисникот НЕ смее да биде веќе Тим Лидер
            if (currentUser == null || currentUser.TeamId.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Немате дозвола да креирате тим. Мора да сте корисник без доделена улога и без тим.");
            }
            
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name")] Team team)
        {
            string currentUserId = User.Identity.GetUserId();
            var currentUser = _userManager.FindById(currentUserId);

            team.TeamLeaderId = currentUserId;

            if (ModelState.IsValid)
            {
                db.Teams.Add(team);
                db.SaveChanges();

                currentUser.TeamId = team.Id;
                _userManager.Update(currentUser);

                if (_userManager.IsInRole(currentUserId, "Employee"))
                {
                    _userManager.RemoveFromRole(currentUserId, "Employee");
                }
                _userManager.AddToRole(currentUserId, "TeamLeader");

                var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                signInManager.SignIn(currentUser, isPersistent: false, rememberBrowser: false);

                return RedirectToAction("Index", "Teams");
            }
            return View(team);
        }


        // GET: Teams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }

            // Само Администратор или Тим Лидер на овој тим може да го измени.
            string currentUserId = User.Identity.GetUserId();
            if (team.TeamLeaderId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Немате дозвола да го измените овој тим.");
            }
            return View(team);
        }

        // POST: Teams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Team team)
        {
            Team originalTeam = db.Teams.AsNoTracking().FirstOrDefault(t => t.Id == team.Id);
            if (originalTeam == null)
            {
                return HttpNotFound();
            }

            string currentUserId = User.Identity.GetUserId();
            if (originalTeam.TeamLeaderId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Немате дозвола да го измените овој тим.");
            }
            team.TeamLeaderId = originalTeam.TeamLeaderId;

            if (ModelState.IsValid)
            {
                db.Entry(team).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(team);
        }

        // GET: Teams/Delete/5
        [Authorize(Roles = "Administrator,TeamLeader")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Include(t => t.Members).FirstOrDefault(t => t.Id == id);
            if (team == null)
            {
                return HttpNotFound();
            }

            string currentUserId = User.Identity.GetUserId();
            if (!User.IsInRole("Administrator") && team.TeamLeaderId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Немате дозвола да го избришете овој тим.");
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,TeamLeader")]
        public ActionResult DeleteConfirmed(int id)
        {
            Team team = db.Teams.Include(t => t.Members).FirstOrDefault(t => t.Id == id);
            if (team == null)
            {
                return HttpNotFound();
            }

            string currentUserId = User.Identity.GetUserId();

            if (!User.IsInRole("Administrator") && team.TeamLeaderId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Немате дозвола да го избришете овој тим.");
            }

            foreach (var member in team.Members.ToList())
            {
                member.TeamId = null;
                _userManager.Update(member);
            }
            string teamLeaderToDeleteRoleId = team.TeamLeaderId;
            var teamLeaderUser = _userManager.FindById(teamLeaderToDeleteRoleId);
            _userManager.RemoveFromRole(teamLeaderUser.Id, "TeamLeader");
            _userManager.AddToRole(teamLeaderUser.Id, "Employee");

            var teamTasks = db.TaskModels.Where(t => t.TeamId == team.Id).ToList();
            foreach (var task in teamTasks)
            {
                task.TeamId = null;
                db.Entry(task).State = EntityState.Modified;
            }
            db.SaveChanges();

            db.Teams.Remove(team);
            db.SaveChanges();

            return RedirectToAction("Index");
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
    }
}
