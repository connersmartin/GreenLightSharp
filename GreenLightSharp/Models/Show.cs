using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLightSharp.Models
{
    public class Show
    {
        public List<Member> Members { get; set; } = new List<Member>();
        public string Band { get; set; }
        public string Id { get; set; }
    }
}