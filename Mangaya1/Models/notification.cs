//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mangaya1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class notification
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string content { get; set; }
        public System.DateTime date { get; set; }
        public bool is_read { get; set; }
    
        public virtual user user { get; set; }
    }
}
