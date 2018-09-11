using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLightSharp.Models
{
    public class Show
    {
        public string Id { get; set; }
        public string Band { get; set; }
        public List<Member> Members { get; set; }
    }
}