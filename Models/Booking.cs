using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Study_Buddy.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Booking_Id { get; set; }

        [Display(Name = "Tutor Name")]
        public string Tutor_name { get; set; }
        [Display(Name = "Student Name")]
        public string Stud_name { get; set; }
        [Display(Name = "Tutor Email")]
        public string Tutor_email { get; set; }

        [Display(Name = "Student Email")]
        public string Stud_email { get; set; }  

        [Display(Name = "Time")]
        public string Book_time { get; set; }
        public int Stud_Id { get; set; } 
        public string Module { get; set; }
        [Display(Name = "Booking Status")]
        public string Student_status { get; set; }
    }
}