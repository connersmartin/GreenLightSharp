using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace GreenLightSharp.Models
{
    public class Member
    {
        public string Id { get; set; }
        [Required(ErrorMessage ="Band Id is required")]
        public string BandId { get; set; }
        public string Status { get; set; }
        public string Instrument { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}