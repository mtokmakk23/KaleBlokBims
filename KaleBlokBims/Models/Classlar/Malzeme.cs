using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaleBlokBims.Models.Classlar
{
    public class Malzeme
    {
        public bool Success;
        public string HataMesaji;
        public string MalzemeKodu;
        public string MalzemeAdi;
        public string MalzemeAciklama;
        public string Birim;
        public string Marka;
        public string SPECODE1;
        public string SPECODE2;
        public string ResimUrl;


        public double PaketKatsayi;
        public double M2Katsayi;
        public double M3Katsayi;
        public double KGKatsayi;


        public double BaseFiyat;
        public string BaseDoviz;
        public double sozlesmeUSD;
        public double sozlesmeEUR;
        public double GuncelUSD;
        public double GuncelEUR;
        public double HesaplanmisBirimFiyatiTL;
        public double NakliyeMasrafiTL;
        public double Kdv;
        public string HesaplamaDetayliAciklama;

        public int NakliyeKartiLref;
        public string NakliyeKodu;
        public string NakliyeAdi;
        public string NakliyeBirimSeti;
    }
}