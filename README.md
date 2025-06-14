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
3.  **Frontend (Rendered):** HTML, CSS, Bootstrap (via CSHTML), JavaScript (jQuery, DataTables.js)

## 💻 Local Installation
1.   **Visual Studio: **Download and open Visual Studio 2022
2.   **Database: **Install SQL Server LocalDB
3.   **Open in Visual Studio: ** Get Started -> Clone a repository, Repository location: https://github.com/spasovskibojan/Team-ToDo.git -> Clone
4.   **Web config: **Solution Explorer -> web.config, change <add name="DefaultConnection" connectionString="Data Source=(LocalDb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\aspnet-IT_Proekt_Proba_Teams-20250607122401.mdf;Initial Catalog=aspnet-IT_Proekt_Proba_Teams-20250607122401;Integrated Security=True" providerName="System.Data.SqlClient" /> to <add name="DefaultConnection" connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=ITProektProbaTeamsDB;Integrated Security=True" providerName="System.Data.SqlClient" />
5.   **Package Manager Console: **type: Update-Package -Reinstall, if it fails type Install-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform and submit all the notes.
6.   **Database check: **View -> Server Explorer, if the DefaultConnection (Task Manager MVC APP) exists, go to Tables and right click -> Show Table Data on AspNetRoles, and add rows with this data: Id:1 && Name:Administrator, Id:2 && Name:TeamLeader, Id:3 && Name:Employee/
7.   **Run: **Run the app and follow the instructions on the homepage


---
