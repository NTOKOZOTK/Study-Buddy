using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Study_Buddy.Models
{
    public class Tutor
    {
        [Key]
        [Required]
        [Display(Name = "Student or Staff Number")]
        public int Tutor_Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Tutor_Name { get; set; }

       
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        [Display(Name = "Year of Studies")]
        [Required]
        public string Level_Of_Study { get; set; }

        [Display(Name = "Admin Approval")]
        public string Status { get; set; }
       

        [Display(Name = "My Availabity")]
        public string Availability { get; set; }

        [Display(Name = "Available Time")]
        public string availableTime { get; set; }

        [Display(Name = " Upload Academic Transcript")]
        public byte[] Binary { get; set; }
       
        [Display(Name = "Level of Study To Tutor")]
        [Required]
        public string Level_2_tut { get; set; }
        [Display(Name = "Faculty")]
        [Required]
        public string Faculty { get; set; }
        [Display(Name = "Department")]
        [Required]
        public string Department { get; set; }

        [Display(Name = "Module To Tutor")]
        [Required]
        public string Module { get; set; }


       
        public virtual ICollection<Student> Students { get; set; }
    }
}