using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Study_Buddy.Models
{
    public class Directions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        [Required]
        [Display(Name ="Student Name")]
        public string Name { get; set; }

        [StringLength(50)]
        [Display(Name = "Student Number")]
        public string StudNumber { get; set; }

        [Display(Name = "Meeting Point")]
        public string MeetingPoint { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Physical Location")]
        public string Location { get; set; }
    }
}