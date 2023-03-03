using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Study_Buddy.Models
{
    public class Student { 


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Stud_Id { get; set; }
        
        [Required]
        [Display(Name ="Student Number")]
        public int Stud_Number { get; set; }

        [Required]
        [Display(Name = "Student Name")]
      
        public string Stud_Name { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Choose Session")]
        public bool Study_Time { get; set; }
        [Display(Name = "Study Times")]
        public string Study_Date { get; set;}
        [Required]
        [Display(Name ="Purpose of Booking")]
        public string pur_booking { get; set; } 

        [Display(Name = "Year of Studies")]
        [Required]
        public string Level_Of_Study { get; set; }
        [Display(Name = "Meeting Point")]
        public string Meeting_Place { get; set; }
        public string Status { get; set; }
        public string Payment_Status { get; set; }
        [Display(Name = "Module")]
        [Required]
        public string Module { get; set; }
        [Display(Name = "Faculty")]
        [Required]
        public string Faculty { get; set; }
        [Display(Name = "Department")]
        [Required]
        public string Department { get; set; }


        
        public virtual Tutor Tutor { get; set; }

    }

}