//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KaleBlokBims.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ANKT_Master
    {
        public int LOGICALREF { get; set; }
        public Nullable<System.DateTime> BaslangicTarihi { get; set; }
        public Nullable<System.DateTime> BitisTarihi { get; set; }
        public string Status { get; set; }
        public string Ekleyen { get; set; }
        public Nullable<System.DateTime> EklemeTarihi { get; set; }
        public string Baslik { get; set; }
        public string AnketTipi { get; set; }
    }
}
