using System;
using BankLib;

Console.WriteLine("=== SYMULACJA SYSTEMU BANKOWEGO ===");

// 1. Symulacja standardowego konta 
Console.WriteLine("\n--- Konto Standardowe ---");
Konto zwykleKonto = new Konto("Jan Kowalski", 1000m);
Console.WriteLine($"Właściciel: {zwykleKonto.Nazwa} | Bilans początkowy: {zwykleKonto.Bilans:C}");

zwykleKonto.Wplata(500m);
Console.WriteLine($"Po wpłacie 500 PLN, bilans: {zwykleKonto.Bilans:C}");

zwykleKonto.Wyplata(200m);
Console.WriteLine($"Po wypłacie 200 PLN, bilans: {zwykleKonto.Bilans:C}");

// Próba wypłaty bez wystarczających środków
try
{
    Console.WriteLine("Próba wypłaty 2000 PLN...");
    zwykleKonto.Wyplata(2000m);
}
catch (Exception ex)
{
    Console.WriteLine($"[Błąd]: {ex.Message}");
}

// Blokowanie konta
Console.WriteLine("\n--- Blokowanie Konta ---");
zwykleKonto.BlokujKonto();
Console.WriteLine($"Czy konto zablokowane? {zwykleKonto.Zablokowane}");

try
{
    Console.WriteLine("Próba wpłaty 100 PLN na zablokowane konto...");
    zwykleKonto.Wplata(100m);
}
catch (Exception ex)
{
    Console.WriteLine($"[Błąd]: {ex.Message}");
}

zwykleKonto.OdblokujKonto();
Console.WriteLine("Konto zostało odblokowane.");


// 2. Symulacja KontoLimit (zakładam, że pozwala wejść na debet)
Console.WriteLine("\n--- Konto z Limitem (KontoLimit) ---");

KontoLimit kontoLimit = new KontoLimit("Anna Nowak", 500m); 
Console.WriteLine($"Właściciel: {kontoLimit.Nazwa} | Bilans początkowy: {kontoLimit.Bilans:C}");

try
{
    Console.WriteLine("Próba wypłaty 800 PLN (wejście w debet)...");
    kontoLimit.Wyplata(800m);
    Console.WriteLine($"Operacja udana. Aktualny bilans: {kontoLimit.Bilans:C}");
}
catch (Exception ex)
{
    Console.WriteLine($"[Błąd]: {ex.Message}");
}


// 3. Symulacja KontoPlus 
Console.WriteLine("\n--- Konto Plus (KontoPlus) ---");
KontoPlus kontoPlus = new KontoPlus("Piotr Wiśniewski", 2000m);
Console.WriteLine($"Właściciel: {kontoPlus.Nazwa} | Bilans początkowy: {kontoPlus.Bilans:C}");

try
{
    kontoPlus.Wplata(1000m);
    Console.WriteLine($"Bilans po wpłacie 1000 PLN: {kontoPlus.Bilans:C}");
    
    kontoPlus.Wyplata(500m);
    Console.WriteLine($"Bilans po wypłacie 500 PLN: {kontoPlus.Bilans:C}");
}
catch (Exception ex)
{
    Console.WriteLine($"[Błąd]: {ex.Message}");
}

Console.WriteLine("\n=== KONIEC SYMULACJI ===");