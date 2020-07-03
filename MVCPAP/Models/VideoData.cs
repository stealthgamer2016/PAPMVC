using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPAP.Models
{
    public class VideoData:Video
    {
        public int upVotes { get; set; }
        public bool Upvoted { get; set; }
    }
}