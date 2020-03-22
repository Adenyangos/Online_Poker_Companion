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
    
    public partial class Hand
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Hand()
        {
            this.BoardActions = new HashSet<BoardAction>();
            this.HandPlayers = new HashSet<HandPlayer>();
        }
    
        public long Id { get; set; }
        public int GameId { get; set; }
        public int Ante { get; set; }
        public System.DateTime DateTimeStart { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BoardAction> BoardActions { get; set; }
        public virtual Game Game { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HandPlayer> HandPlayers { get; set; }
    }
}
