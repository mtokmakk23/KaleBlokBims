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
    
    public partial class AdminKullaniciYetkisi
    {
        public long LOGICALREF { get; set; }
        public Nullable<int> KullaniciID { get; set; }
        public Nullable<bool> BayiAdinaGiris { get; set; }
        public Nullable<bool> KullaniciTanimlama { get; set; }
        public Nullable<bool> SiparisAktarma { get; set; }
        public Nullable<bool> RaporlariGorme { get; set; }
        public Nullable<bool> YoneticiRaporlariniGorme { get; set; }
        public Nullable<bool> SikayetYetkisi { get; set; }
        public Nullable<bool> TeklifYetkisi { get; set; }
        public Nullable<bool> EvrakYetkisi { get; set; }
        public Nullable<bool> LogKayitlariGorme { get; set; }
    }
}
