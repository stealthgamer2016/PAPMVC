//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVCPAP
{
    using System;
    using System.Collections.Generic;
    using MVCPAP.Models;

    public partial class Video
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Video()
        {
            this.Comment = new HashSet<Comment>();
        }
    
        public int id { get; set; }
        public string title { get; set; }
        public string username { get; set; }
        public Nullable<int> discriminator { get; set; }
        public string videoFile { get; set; }
        public string thumbnail { get; set; }
        public Nullable<int> upvotes { get; set; }
        public Nullable<int> downvotes { get; set; }
        public Nullable<System.DateTime> publishedAt { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual User User { get; set; }
    }
}