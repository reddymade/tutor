using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InstantTutors.Areas.Student.ViewModels
{
    public class SessionViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Session Title/Subject")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Tutor")]
        public string Tutor { get; set; }

        [Required]
        [Display(Name = "Session Description")]
        public string Description { get; set; }

        [Display(Name = "Questions/Comments/Concerns")]
        public string Concerns { get; set; }
        
        [Required]
        [Display(Name = "Preferred Method of Communication")]
        public string CommunicationMethod { get; set; }

        public string UserId { get; set; } //FK
        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; } 
        public ApplicationUser User { get; set; }
        public List<SessionScheduleViewModel> SessionSchedules { get; set; }
        public List<SelectListItem> Tutors  { get; set; }

        public bool IsRequestComingFromTutor { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime? StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime? EndDate { get; set; }
    }
}