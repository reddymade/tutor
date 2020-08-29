using InstantTutors.Models;
using InstantTutors.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InstantTutors.Areas.Tutor.ViewModels
{
    public class TutorSessionViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Session Title/Subject")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Student")]
        public string Student { get; set; }

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
        public List<TutorSessionScheduleViewModel> SessionSchedules { get; set; }
        public List<SelectListItem> Students  { get; set; }
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

    public class TutorSessionScheduleViewModel
    {
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        public int Time { get; set; }
        public List<TimeViewModel> Timing { get; set; }
        public int SessionId { get; set; } //FK
        public string UserId { get; set; } //FK
        public DateTime CreatedDate { get; set; }

        public DateTime SelectedDate { get; set; }
    }

    
}