//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ISAD251_Coursework.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public Order()
        {
            this.IsDelivered = "False";
        }
    
        public int Id { get; set; }
        public System.DateTime OrderTime { get; set; }
        public Nullable<int> Quantity { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public string IsDelivered { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual User User { get; set; }
    }
}
