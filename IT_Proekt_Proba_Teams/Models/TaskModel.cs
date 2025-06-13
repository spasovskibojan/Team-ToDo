using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace IT_Proekt_Proba_Teams.Models
{
    public class TaskModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Насловот е задолжителен.")]
        [StringLength(50, ErrorMessage = "Насловот не може да биде подолг од 50 карактери.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Приоритетот е задолжителен.")]
        public string Priority { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Крајниот рок е задолжителен.")]
        public DateTime DueDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [DisplayName("Completed")]
        public bool IsCompleted { get; set; }

        // -- Dependencies --

        [Required]
        [Display(Name = "Creator")]
        public string CreatedByUserId { get; set; }
        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser CreatedByUser { get; set; }

        public string AssignedToUserId { get; set; }
        [ForeignKey("AssignedToUserId")]
        public virtual ApplicationUser AssignedToUser { get; set; }

        public int? TeamId { get; set; }
        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }
    }
}