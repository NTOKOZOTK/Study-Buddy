using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Study_Buddy.Models
{
    public class Reply
    {
        [Key]
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string ReplyFrom { get; set; }
        [Required]
        public string ReplyMessage { get; set; }
        public DateTime ReplyDateTime { get; set; }
    }
}