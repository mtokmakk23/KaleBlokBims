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
    
    public partial class BayiKullanicilari
    {
        public long LOGICALREF { get; set; }
        public string MailAdresi { get; set; }
        public string Sifre { get; set; }
        public string AdiSoyadi { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> AdminMi { get; set; }
        public string BayiKodu { get; set; }
        public string BayiAdi { get; set; }
        public string GSM { get; set; }
        public Nullable<System.DateTime> SifreDegistirmeTarihi { get; set; }
        public string GeciciSifre { get; set; }
    }
}
