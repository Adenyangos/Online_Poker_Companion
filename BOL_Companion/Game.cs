//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BOL_Companion
{
    using System;
    using System.Collections.Generic;
    
    public partial class Game
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Game()
        {
            this.Hands = new HashSet<Hand>();
        }
    
        public int Id { get; set; }
        public bool Tournament { get; set; }
        public Nullable<int> NumPlayers { get; set; }
        public System.DateTime DateTimeStart { get; set; }
        public Nullable<decimal> BuyIn { get; set; }
        public Nullable<int> ReBuys { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Hand> Hands { get; set; }
    }
}
