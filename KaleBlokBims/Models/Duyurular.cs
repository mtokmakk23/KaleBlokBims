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
    
    public partial class Duyurular
    {
        public int LOGICALREF { get; set; }
        public string Baslik { get; set; }
        public string Icerik { get; set; }
        public System.DateTime BaslangicTarihi { get; set; }
        public System.DateTime BitisTarihi { get; set; }
        public bool AcilirMesajMi { get; set; }
        public string AcilirMesajGenisligi { get; set; }
        public bool AdminDuyurusuMu { get; set; }
        public bool BayiDuyurusuMu { get; set; }
    }
}
