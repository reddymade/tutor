using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InstantTutors.Models.ViewModels
{
    public class MessageCenterViewModel
    {
        [Required]
        [Display(Name = "Message Body")]
        public string MessageBody { get; set; }
        [Required]
        [Display(Name = "Tutor")]
        public string Tutor { get; set; }

       
        [Display(Name = "Student")]
        public string Student { get; set; }

        public List<SelectListItem> Students { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Tutors { get; set; } = new List<SelectListItem>();
        public List<Messages> Messages { get; set; } = new List<Messages>();
    }
    public class Messages
    {
        public string MessageContent { get; set; }
        public string MessageTo { get; set; }
        public string MessageFrom { get; set; }
        public DateTime CreatedDate { get; set; }
        public string MessageId { get; set; }

    }
}