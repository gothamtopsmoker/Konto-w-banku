using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankLib.Tests
{
    [TestClass]
    public sealed class KontoPlusTests
    {
        [TestMethod]
        public void Konstruktor_Z_UjemnymLimitem_ZglaszaWyjatek()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new KontoPlus("Jan Kowalski", 100m, -50m));
        }

        [TestMethod]
        public void LimitDebetowy_ZmianaNaUjemny_ZglaszaWyjatek()
        {
            var konto = new KontoPlus("Jan Kowalski", 100m, 50m);
            Assert.Throws<ArgumentOutOfRangeException>(() => konto.LimitDebetowy = -10m);
        }

        [TestMethod]
        public void Bilans_Z_DostepnymLimitem_ZwracaSumeBilansuILimitu()
        {
            var konto = new KontoPlus("Jan Kowalski", 100m, 50m);
            Assert.AreEqual(150m, konto.Bilans);
        }

        [TestMethod]
        public void Wyplata_WZasieguLimituDebetowego_ZmniejszaBilansIZablokowuje()
        {
            var konto = new KontoPlus("Jan Kowalski", 100m, 50m);
            
            konto.Wyplata(130m);
            
            Assert.AreEqual(0m, konto.Bilans); // dostepnyLimit = false, wiec Bilans = 0
            Assert.IsTrue(konto.Zablokowane);
            
            // bilans wewnetrzny to -30m
        }

        [TestMethod]
        public void Wyplata_PrzekraczajacaLimitDebetowy_ZglaszaWyjatek()
        {
            var konto = new KontoPlus("Jan Kowalski", 100m, 50m);
            
            var ex = Assert.Throws<InvalidOperationException>(() => konto.Wyplata(160m));
            Assert.AreEqual("Brak wystarczających środków na koncie.", ex.Message);
        }

        [TestMethod]
        public void Wplata_PoDebecie_CzesciowaWplata_KontoNadalZablokowane()
        {
            var konto = new KontoPlus("Jan Kowalski", 100m, 50m);
            konto.Wyplata(130m); // Bilans wewnetrzny = -30m, zablokowane
            
            konto.Wplata(10m); // Bilans wewnetrzny = -20m
            
            Assert.IsTrue(konto.Zablokowane);
        }

        [TestMethod]
        public void Wplata_PoDebecie_CalkowitaWplata_KontoOdblokowane()
        {
            var konto = new KontoPlus("Jan Kowalski", 100m, 50m);
            konto.Wyplata(130m); // Bilans wewnetrzny = -30m, zablokowane
            
            konto.Wplata(40m); // Bilans wewnetrzny = 10m
            
            Assert.IsFalse(konto.Zablokowane);
            Assert.AreEqual(60m, konto.Bilans); // 10m + 50m limitu
        }

        [TestMethod]
        public void Wplata_ZablokowaneZInnegoPowodu_ZglaszaWyjatek()
        {
            var konto = new KontoPlus("Jan Kowalski", 100m, 50m);
            konto.BlokujKonto();
            
            var ex = Assert.Throws<InvalidOperationException>(() => konto.Wplata(50m));
            Assert.AreEqual("Operacja niedozwolona: konto jest zablokowane z innego powodu.", ex.Message);
        }
    }
}