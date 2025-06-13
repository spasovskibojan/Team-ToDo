using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IT_Proekt_Proba_Teams.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IT_Proekt_Proba_Teams.Controllers
{
    [Authorize]
    public class TaskModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> _userManager;

        public TaskModelsController()
        {
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        //GET: TaskModels
        public ActionResult Index()
        {
            string currentUserId = User.Identity.GetUserId();
            var currentUser = _userManager.FindById(currentUserId);

            IQueryable<TaskModel> taskModels = db.TaskModels
                                                .Include(t => t.AssignedToUser)
                                                .Include(t => t.CreatedByUser)
                                                .Include(t => t.Team);

            if (User.IsInRole("Administrator"))
            {
                taskModels = taskModels.Where(t => false);
            }
            else if (User.IsInRole("TeamLeader"))
            {
                var teamLeaderTeamId = currentUser.TeamId;

                if (teamLeaderTeamId.HasValue)
                {
                    var teamMemberAndLeaderIds = db.Users
                                                    .Where(u => u.TeamId == teamLeaderTeamId)
                                                    .Select(u => u.Id)
                                                    .ToList();

                    taskModels = taskModels.Where(t =>
                        (t.CreatedByUserId == currentUserId && t.AssignedToUserId == currentUserId) ||
                        (t.CreatedByUserId == currentUserId && teamMemberAndLeaderIds.Contains(t.AssignedToUserId))
                    );
                }
                else
                {
                    taskModels = taskModels.Where(t => t.CreatedByUserId == currentUserId && t.AssignedToUserId == currentUserId);
                }
            }
            else if (User.IsInRole("Employee"))
            {
                taskModels = taskModels.Where(t => t.CreatedByUserId == currentUserId || t.AssignedToUserId == currentUserId);
            }
            else
            {
                taskModels = taskModels.Where(t => false);
            }

            //taskModels = taskModels.OrderBy(t => t.DueDate);
                //OrderByDescending(t => t.Priority == "Critical")
                //                   .ThenByDescending(t => t.Priority == "High")
                //                   .ThenBy(t => t.DueDate);

            return View(taskModels.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkTaskAsCompleted(int id, bool isCompleted)
        {
            string currentUserId = User.Identity.GetUserId();
            TaskModel task = db.TaskModels.Find(id);

            task.IsCompleted = isCompleted;
            db.Entry(task).State = EntityState.Modified; 

            try
            {
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Грешка при зачувување на промените: " + ex.Message });
            }
        }
        // GET: TaskModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskModel taskModel = db.TaskModels.Find(id);
            if (taskModel == null)
            {
                return HttpNotFound();
            }
            return View(taskModel);
        }

        [Authorize(Roles = "TeamLeader,Employee")]
        public ActionResult Create()
        {
            string currentUserId = User.Identity.GetUserId();
            var currentUser = _userManager.FindById(currentUserId);

            var model = new TaskModel();

            model.CreatedByUserId = currentUserId;
            model.DueDate = DateTime.Today;

            if (currentUser.TeamId.HasValue)
            {
                model.TeamId = currentUser.TeamId;
            }
            ViewBag.Priority = new SelectList(new List<string> { "Low", "Medium", "High", "Critical" });

            List<ApplicationUser> assignedToUsersList = new List<ApplicationUser>();
            if (User.IsInRole("TeamLeader") && currentUser.TeamId.HasValue)
            {
                assignedToUsersList = db.Users.Where(u => u.TeamId == currentUser.TeamId && u.Id != currentUserId).ToList();
            }
            ViewBag.AssignedToUserId = new SelectList(assignedToUsersList, "Id", "Email");

            return View(model); 
        }

        // POST: TaskModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TeamLeader,Employee")]
        public ActionResult Create(TaskModel taskModel)
        {
            string currentUserId = User.Identity.GetUserId();
            var currentUser = _userManager.FindById(currentUserId);

            taskModel.CreatedByUserId = currentUserId;

            if (currentUser.TeamId.HasValue)
            {
                taskModel.TeamId = currentUser.TeamId;
            }
            else
            {
                taskModel.TeamId = null;
            }

            if (User.IsInRole("TeamLeader"))
            {
                string assignedToId = Request.Form["AssignedToUserId"];
                if (!string.IsNullOrEmpty(assignedToId))
                {
                    var assignedUser = db.Users.Find(assignedToId);
                    if (assignedUser != null && assignedUser.TeamId == currentUser.TeamId)
                    {
                        taskModel.AssignedToUserId = assignedToId;
                    }
                    else
                    {
                        ModelState.AddModelError("AssignedToUserId", "Избраниот корисник не е валиден член на вашиот тим.");
                        taskModel.AssignedToUserId = null;
                    }
                }
                else
                {
                    taskModel.AssignedToUserId = currentUserId;
                }
            }
            else if (User.IsInRole("Employee"))
            {
                taskModel.AssignedToUserId = currentUserId;
            }
            else
            {
                taskModel.AssignedToUserId = null;
            }

            taskModel.IsCompleted = false; 

            if (ModelState.IsValid)
            {
                db.TaskModels.Add(taskModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Priority = new SelectList(new List<string> { "Low", "Medium", "High", "Critical" }, taskModel.Priority);

            List<ApplicationUser> assignedToUsersListForPost = new List<ApplicationUser>();
            if (User.IsInRole("TeamLeader") && currentUser.TeamId.HasValue)
            {
                assignedToUsersListForPost = db.Users.Where(u => u.TeamId == currentUser.TeamId && u.Id != currentUserId).ToList();
            }
            ViewBag.AssignedToUserId = new SelectList(assignedToUsersListForPost, "Id", "Email", taskModel.AssignedToUserId);

            ViewBag.TeamId = new SelectList(new List<Team>());


            return View(taskModel);
        }

        // GET: TaskModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskModel taskModel = db.TaskModels.Find(id);
            if (taskModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "Email", taskModel.AssignedToUserId);
            ViewBag.CreatedByUserId = new SelectList(db.Users, "Id", "Email", taskModel.CreatedByUserId);
            ViewBag.TeamId = new SelectList(db.Teams, "Id", "Name", taskModel.TeamId);
            List<string> allPriorities = new List<string> { "Low", "Medium", "High", "Critical" };
            List<string> selectablePriorities = allPriorities.Where(p => p != taskModel.Priority).ToList();
            ViewBag.Priority = new SelectList(selectablePriorities);
            //ViewBag.Priority = new SelectList(new List<string> { "Low", "Medium", "High", "Critical" }, taskModel.Priority);
            return View(taskModel);
        }

        // POST: TaskModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( TaskModel taskModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(taskModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "Email", taskModel.AssignedToUserId);
            ViewBag.CreatedByUserId = new SelectList(db.Users, "Id", "Email", taskModel.CreatedByUserId);
            ViewBag.TeamId = new SelectList(db.Teams, "Id", "Name", taskModel.TeamId);
            ViewBag.Priority = new SelectList(new List<string> { "Low", "Medium", "High", "Critical" }, taskModel.Priority);
            return View(taskModel);
        }

        // GET: TaskModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskModel taskModel = db.TaskModels.Find(id);
            if (taskModel == null)
            {
                return HttpNotFound();
            }
            return View(taskModel);
        }

        // POST: TaskModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TaskModel taskModel = db.TaskModels.Find(id);
            db.TaskModels.Remove(taskModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
