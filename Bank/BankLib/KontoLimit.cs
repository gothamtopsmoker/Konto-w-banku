using System;

namespace BankLib
{
    public class KontoLimit
    {
        private Konto konto;
        private decimal limitDebetowy;
        private bool dostepnyLimit = true;
        private bool zablokowanePrzezDebet = false;

        public decimal LimitDebetowy
        {
            get => limitDebetowy;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Limit debetowy nie może być ujemny.");
                }
                limitDebetowy = value;
            }
        }

        public string Nazwa => konto.Nazwa;
        
        public decimal Bilans => dostepnyLimit ? konto.Bilans + limitDebetowy : 0;
        
        public bool Zablokowane => konto.Zablokowane;

        public KontoLimit(string klient, decimal bilansNaStart = 0, decimal limitDebetowy = 0)
        {
            konto = new Konto(klient, bilansNaStart);
            LimitDebetowy = limitDebetowy;
        }

        public void Wplata(decimal kwota)
        {
            if (kwota <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(kwota), "Kwota wpłaty musi być większa od zera.");
            }

            if (konto.Zablokowane && !zablokowanePrzezDebet)
            {
                throw new InvalidOperationException("Operacja niedozwolona: konto jest zablokowane z innego powodu.");
            }

            bool bylZablokowanyPrzezDebet = zablokowanePrzezDebet;
            
            if (bylZablokowanyPrzezDebet)
            {
                konto.OdblokujKonto();
            }

            try
            {
                konto.Wplata(kwota);
            }
            finally
            {
                if (bylZablokowanyPrzezDebet)
                {
                    if (konto.Bilans > 0)
                    {
                        zablokowanePrzezDebet = false;
                        dostepnyLimit = true;
                    }
                    else
                    {
                        konto.BlokujKonto();
                    }
                }
            }
        }

        public void Wyplata(decimal kwota)
        {
            if (konto.Zablokowane)
            {
                throw new InvalidOperationException("Operacja niedozwolona: konto jest zablokowane.");
            }

            if (kwota <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(kwota), "Kwota wypłaty musi być większa od zera.");
            }

            if (kwota > konto.Bilans)
            {
                if (dostepnyLimit && kwota <= konto.Bilans + LimitDebetowy)
                {
                    konto.bilans -= kwota;
                    dostepnyLimit = false;
                    zablokowanePrzezDebet = true;
                    konto.BlokujKonto();
                }
                else
                {
                    throw new InvalidOperationException("Brak wystarczających środków na koncie.");
                }
            }
            else
            {
                konto.Wyplata(kwota);
            }
        }

        public void BlokujKonto()
        {
            konto.BlokujKonto();
        }

        public void OdblokujKonto()
        {
            konto.OdblokujKonto();
        }
    }
}