﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class IZOKALEPORTALEntities : DbContext
    {
        public IZOKALEPORTALEntities()
            : base("name=IZOKALEPORTALEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AdminKullanicilari> AdminKullanicilari { get; set; }
        public virtual DbSet<BayiKullanicilari> BayiKullanicilari { get; set; }
        public virtual DbSet<KayitliAdresler> KayitliAdresler { get; set; }
        public virtual DbSet<SiparisBasliklari> SiparisBasliklari { get; set; }
        public virtual DbSet<SiparisIcerikleri> SiparisIcerikleri { get; set; }
    }
}
