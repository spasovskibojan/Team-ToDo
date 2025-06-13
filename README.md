# 🚀 Team ToDo

A comprehensive web application for managing and delegating tasks within teams. Organize work efficiently, collaborate, and track deadlines with ease.

## ✨ Key Features

1.  **Task Management:** Create, view, edit, and delete tasks.
2.  **Delegation:** Team Leaders can delegate tasks to team members.
3.  **Priorities:** Assign priorities (Critical, High, Medium, Low) with visual highlighting.
4.  **Deadlines:** Track Due Dates; overdue tasks are visually marked.
5.  **Completion:** Mark tasks as completed with one click.
6.  **Team Management:** Create and manage teams (Team Leaders and Employees).
7.  **Role-Based Access:** Clear roles (Administrator, Team Leader, Employee) with specific permissions.
8.  **Intuitive UI:** DataTables for filtering and sorting tasks.

## 👥 User Roles

1.  **Administrator:** Full system access, views statistics, manages user roles.
2.  **Team Leader:** Creates and manages their team, adds/removes employees, creates/delegates tasks, views team tasks.
3.  **Employee:** Creates personal tasks, views delegated tasks, can create a new team if unassigned.

## 🚀 How to Use

1.  **Register:** Sign up on the platform. New users start as "Employee".
2.  **Create Team (Employees without a team):** Go to "Teams" -> "Create Team" to become a Team Leader.
3.  **Add Team Members (Team Leaders):** In "Teams" -> Team Details, add members by email.
4.  **Create Task:**
    * **Employees:** "Tasks" -> "Create New Task" (personal).
    * **Team Leaders:** "Tasks" -> "Create New Task" (assign to self or delegate to team member).
5.  **Manage Tasks:** On "Tasks" page, complete tasks (checkbox), or use options for edit/details/delete.
6.  **Admin Panel (Administrators):** Access "Работна Табла" (Dashboard) and "Кориснички улоги" (User Roles) for statistics and role management.

## 🛠️ Technologies Used

1.  **Backend:** C# .NET Framework (ASP.NET MVC with Razor Views)
2.  **Database:** SQL Server (LocalDB/Express for development)
3.  **Frontend (Rendered):** HTML, CSS (via CSHTML), JavaScript (jQuery, DataTables.js)

## 💻 Local Installation

1.  **Clone:** `git clone https://github.com/your-username/your-repo-name.git`
2.  **Open in Visual Studio:** Open the `.sln` file.
3.  **Restore NuGet Packages.**
4.  **Database:** Ensure SQL Server LocalDB/Express is installed. Update `Web.config` connection string. Run `Update-Database` in Package Manager Console if using Code First Migrations.
5.  **Run:** Press `F5` in Visual Studio.

## 🤝 Contributing

1.  Fork the repository.
2.  Create a new branch (`git checkout -b feature/your-feature-name`).
3.  Make your changes.
4.  Commit your changes (`git commit -m 'Add new feature'`).
5.  Push to the branch (`git push origin feature/your-feature-name`).
6.  Open a Pull Request.

---
