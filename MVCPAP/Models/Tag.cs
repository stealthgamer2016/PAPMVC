using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPAP.Models
{
    public class Tag
    {

        public string text { get; set; }

        public virtual Video video { get; set; }
    }
}