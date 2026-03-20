using BankLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
namespace BankLib.Tests;

[TestClass]
public sealed class KontoTests
{
    [TestMethod]
    public void Konstruktor_Z_PoprawnymiDanymi_TworzyObiekt()
    {
        var konto = new Konto("Jan Kowalski", 100m);

        Assert.AreEqual("Jan Kowalski", konto.Nazwa);
        Assert.AreEqual(100m, konto.Bilans);
        Assert.IsFalse(konto.Zablokowane);
    }

    [TestMethod]
    public void Wplata_PrawidlowaKwota_ZwiekszaBilans()
    {
        var konto = new Konto("Jan Kowalski", 100m);

        konto.Wplata(50m);

        Assert.AreEqual(150m, konto.Bilans);
    }

    [TestMethod]
    public void Wplata_KwotaUjemna_ZglaszaWyjatek()
    {
        var konto = new Konto("Jan Kowalski", 100m);

        Assert.Throws<ArgumentOutOfRangeException>(() => konto.Wplata(-10m));
    }

    [TestMethod]
    public void Wplata_KontoZablokowane_ZglaszaWyjatek()
    {
        var konto = new Konto("Jan Kowalski", 100m);
        konto.BlokujKonto();

        var ex = Assert.Throws<InvalidOperationException>(() => konto.Wplata(50m));
        Assert.AreEqual("Operacja niedozwolona: konto jest zablokowane.", ex.Message);
    }

    [TestMethod]
    public void Wyplata_PrawidlowaKwota_ZmniejszaBilans()
    {
        var konto = new Konto("Jan Kowalski", 100m);

        konto.Wyplata(50m);

        Assert.AreEqual(50m, konto.Bilans);
    }

    [TestMethod]
    public void Wyplata_KwotaUjemna_ZglaszaWyjatek()
    {
        var konto = new Konto("Jan Kowalski", 100m);

        Assert.Throws<ArgumentOutOfRangeException>(() => konto.Wyplata(-10m));
    }

    [TestMethod]
    public void Wyplata_KwotaWiekszaOdBilansu_ZglaszaWyjatek()
    {
        var konto = new Konto("Jan Kowalski", 100m);

        var ex = Assert.Throws<InvalidOperationException>(() => konto.Wyplata(150m));
        Assert.AreEqual("Brak wystarczających środków na koncie.", ex.Message);
    }

    [TestMethod]
    public void Wyplata_KontoZablokowane_ZglaszaWyjatek()
    {
        var konto = new Konto("Jan Kowalski", 100m);
        konto.BlokujKonto();

        var ex = Assert.Throws<InvalidOperationException>(() => konto.Wyplata(50m));
        Assert.AreEqual("Operacja niedozwolona: konto jest zablokowane.", ex.Message);
    }

    [TestMethod]
    public void BlokujKonto_ZmieniaStanNaZablokowane()
    {
        var konto = new Konto("Jan Kowalski", 100m);

        konto.BlokujKonto();

        Assert.IsTrue(konto.Zablokowane);
    }

    [TestMethod]
    public void OdblokujKonto_ZmieniaStanNaOdblokowane()
    {
        var konto = new Konto("Jan Kowalski", 100m);
        konto.BlokujKonto();

        konto.OdblokujKonto();

        Assert.IsFalse(konto.Zablokowane);
    }
}
