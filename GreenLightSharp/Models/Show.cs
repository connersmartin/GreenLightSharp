using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace GreenLightSharp.Models
{
    public class Show
    {
        [Required(ErrorMessage = "Band Name is required")]
        public string Band { get; set; }
        public string Id { get; set; }
        public List<Member> Members { get; set; } = new List<Member>();
        [Range(2, 100, ErrorMessage = "You must have at least 2 members")]
        public string ShowStatus { get; set; }
        public int Size { get; set; }
    }
}