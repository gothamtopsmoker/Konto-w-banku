using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BankLib;

namespace BankLib.Tests
{
    [TestClass]
    public sealed class KontoLimitTests
    {
        [TestMethod]
        public void Konstruktor_PoprawneDane_UstawiaWlasciwosci()
        {
            var konto = new KontoLimit("Piotr Nowacki", 100m, 50m);
            
            Assert.AreEqual("Piotr Nowacki", konto.Nazwa);
            Assert.AreEqual(150m, konto.Bilans); // 100m bazowego + 50m limitu
            Assert.AreEqual(50m, konto.LimitDebetowy);
            Assert.IsFalse(konto.Zablokowane);
        }

        [TestMethod]
        public void Konstruktor_Z_UjemnymLimitem_ZglaszaWyjatek()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new KontoLimit("Jan Kowalski", 100m, -50m));
        }

        [TestMethod]
        public void LimitDebetowy_ZmianaNaUjemny_ZglaszaWyjatek()
        {
            var konto = new KontoLimit("Jan Kowalski", 100m, 50m);
            Assert.Throws<ArgumentOutOfRangeException>(() => konto.LimitDebetowy = -10m);
        }

        [TestMethod]
        public void Wyplata_WZakresiePodstawowychSrodkow_ZmniejszaBilansBezBlokowania()
        {
            var konto = new KontoLimit("Jan Kowalski", 100m, 50m);
            
            konto.Wyplata(40m);
            
            Assert.AreEqual(110m, konto.Bilans); // 60m bazowego + 50m limitu
            Assert.IsFalse(konto.Zablokowane);
        }

        [TestMethod]
        public void Wyplata_WZasieguLimituDebetowego_ZmniejszaBilansIZablokowuje()
        {
            var konto = new KontoLimit("Jan Kowalski", 100m, 50m);
            
            konto.Wyplata(130m);
            
            Assert.AreEqual(0m, konto.Bilans); // dostepnyLimit = false, wiec Bilans = 0
            Assert.IsTrue(konto.Zablokowane);
        }

        [TestMethod]
        public void Wyplata_PrzekraczajacaLimitDebetowy_ZglaszaWyjatek()
        {
            var konto = new KontoLimit("Jan Kowalski", 100m, 50m);
            
            var ex = Assert.Throws<InvalidOperationException>(() => konto.Wyplata(160m));
            Assert.AreEqual("Brak wystarczających środków na koncie.", ex.Message);
        }

        [TestMethod]
        public void Wplata_PoDebecie_CzesciowaWplata_KontoNadalZablokowane()
        {
            var konto = new KontoLimit("Jan Kowalski", 100m, 50m);
            konto.Wyplata(130m); 
            
            konto.Wplata(10m); 
            
            Assert.IsTrue(konto.Zablokowane);
        }

        [TestMethod]
        public void Wplata_PoDebecie_CalkowitaWplata_KontoOdblokowane()
        {
            var konto = new KontoLimit("Jan Kowalski", 100m, 50m);
            konto.Wyplata(130m); 
            
            konto.Wplata(40m); 
            
            Assert.IsFalse(konto.Zablokowane);
            Assert.AreEqual(60m, konto.Bilans); // 10m bazowego + 50m limitu
        }

        [TestMethod]
        public void Wplata_ZablokowaneZInnegoPowodu_ZglaszaWyjatek()
        {
            var konto = new KontoLimit("Jan Kowalski", 100m, 50m);
            konto.BlokujKonto();
            
            var ex = Assert.Throws<InvalidOperationException>(() => konto.Wplata(50m));
            Assert.AreEqual("Operacja niedozwolona: konto jest zablokowane z innego powodu.", ex.Message);
        }

        [TestMethod]
        public void BlokujIOdblokuj_ZmieniaStanZablokowania()
        {
            var konto = new KontoLimit("Jan Kowalski", 100m, 50m);
            
            konto.BlokujKonto();
            Assert.IsTrue(konto.Zablokowane);
            
            konto.OdblokujKonto();
            Assert.IsFalse(konto.Zablokowane);
            
            // Weryfikacja po odblokowaniu - można znów wpłacać pod limit
            konto.Wplata(10m);
            Assert.AreEqual(160m, konto.Bilans); 
        }

        [TestMethod]
        public void DegraduajDoKontaStandardowego_ZDodatnimBilansem_ZwracaPoprawnieIZabezpieczaKopia()
        {
            var kontoLimit = new KontoLimit("Piotr Nowacki", 100m, 50m);
            var standardowe = kontoLimit.DegraduajDoKontaStandardowego();

            Assert.IsNotNull(standardowe);
            Assert.AreEqual("Piotr Nowacki", standardowe.Nazwa);
            // 100 bazowego kapitału trafia do nowego konta
            Assert.AreEqual(100m, standardowe.Bilans);

            // Stary obiekt powinien być zablokowany i po wyzerowaniu bazy mieć bilans = limitowi (gdyż Bilans sumuje kapitał + limit)
            Assert.IsTrue(kontoLimit.Zablokowane);
            Assert.AreEqual(50m, kontoLimit.Bilans); // kapitał 0 + 50 limit
        }

        [TestMethod]
        public void DegraduajDoKontaStandardowego_ZUjemnymBilansem_RzucaWyjatek()
        {
            var kontoLimit = new KontoLimit("Piotr Nowacki", 0m, 50m);
            kontoLimit.Wyplata(30m); // Powoduje że rezerwa bazowa w konto wpada na minusie = brak spłaty limitu

            Assert.Throws<InvalidOperationException>(() => kontoLimit.DegraduajDoKontaStandardowego());
        }
    }
}