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
    
    public partial class TeklifIcerikleri
    {
        public long LOGICALREF { get; set; }
        public Nullable<long> BaslikLREF { get; set; }
        public Nullable<int> LINETYPE { get; set; }
        public string FiyatListesi { get; set; }
        public string MalzemeKodu { get; set; }
        public string MalzemeAdi { get; set; }
        public string Birimi { get; set; }
        public string AnaGrup { get; set; }
        public string AltGrup { get; set; }
        public Nullable<double> Miktar { get; set; }
        public Nullable<double> HesaplanmisBirimFiyatiTL { get; set; }
        public Nullable<double> Kdv { get; set; }
        public string HesaplamaDetayliAciklama { get; set; }
        public Nullable<double> BaseFiyat { get; set; }
        public string BaseDoviz { get; set; }
        public Nullable<double> NakliyeFiyatiTL { get; set; }
        public Nullable<double> GuncelUSD { get; set; }
        public Nullable<double> GuncelEUR { get; set; }
        public Nullable<double> SabitUSD { get; set; }
        public Nullable<double> SabitEUR { get; set; }
        public Nullable<System.DateTime> EklenmeTarihi { get; set; }
        public Nullable<long> IndiriminUygulanacagiLOGICALREF { get; set; }
        public Nullable<double> IndirimTutari { get; set; }
        public string IndirimAciklamasi { get; set; }
        public Nullable<bool> Editable { get; set; }
        public Nullable<long> NakliyeninUygulanacagiLref { get; set; }
        public Nullable<long> NakliyeKartiLref { get; set; }
        public string NakliyeKodu { get; set; }
        public string NakliyeAdi { get; set; }
        public string NakliyeBirimSeti { get; set; }
        public Nullable<bool> SistemKalemiMi { get; set; }
    }
}