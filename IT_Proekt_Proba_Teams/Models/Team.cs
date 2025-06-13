using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IT_Proekt_Proba_Teams.Models
{
	public class Team
	{
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name="Team Name")]
        public string Name { get; set; }

        // -- Dependencies --

        public string TeamLeaderId { get; set; }
        public virtual ApplicationUser TeamLeader { get; set; }

        public virtual ICollection<ApplicationUser> Members { get; set; }
        public Team()
        {
            Members = new HashSet<ApplicationUser>();
        }
    }
}