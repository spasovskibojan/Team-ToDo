using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IT_Proekt_Proba_Teams.Models
{
	public class AdminDashboardViewModel
	{
        public int TotalUsers { get; set; }
        public int TotalTeams { get; set; }
        public int TotalTasks { get; set; }
        public int TotalCompletedTasks { get; set; }
        public int TotalActiveTasks { get; set; }
        public int TotalTeamLeaders { get; set; }
        public int TotalEmployees { get; set; }
    }
}