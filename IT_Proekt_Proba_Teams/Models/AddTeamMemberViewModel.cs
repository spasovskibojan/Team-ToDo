using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IT_Proekt_Proba_Teams.Models
{
    public class AddTeamMemberViewModel
    {
        [Required(ErrorMessage = "Е-поштата на членот е задолжителна.")]
        [EmailAddress(ErrorMessage = "Внесете валидна е-пошта.")]
        [Display(Name = "Е-пошта на Член")]
        public string Email { get; set; }

        public int TeamId { get; set; }
    }
}