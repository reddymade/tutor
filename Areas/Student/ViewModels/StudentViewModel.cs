using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InstantTutors.Areas.Student.ViewModels
{
    public class StudentViewModel
    {
        public string UserId { get; set; } //FK

        public DateTime? CreatedDate { get; set; }

        public ApplicationUser User { get; set; }

        public int CurrentPage { get; set; }
        public int PerPage { get; set; }

        public List<Sessions> SessionsList { get; set; }
    }
}