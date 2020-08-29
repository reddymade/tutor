using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstantTutors.Models
{
    [Table("Sessions")]
    public class Sessions
    {
        [Key][Required]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } //FK
        public string Title { get; set; }
        public string Description { get; set; }
        public string Concerns { get; set; }
        public string CommunicationMethod { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public ICollection<SessionSchedule> SessionSchedule { get; set; }

        public string TutorUserId { get; set; }

        public string TutorName { get; set; }

        public int Status { get; set; }

        public string ApproveDeclineBy { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }


    }
}