using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace GreenLightSharp.Models
{
    public class Show
    {
        public List<Member> Members { get; set; } = new List<Member>();
        [Required(ErrorMessage = "Band Name is required")]
        public string Band { get; set; }
        public string Id { get; set; }
        [Range(2, 100, ErrorMessage = "You must have at least 2 members")]
        public int Size { get; set; }
    }
}