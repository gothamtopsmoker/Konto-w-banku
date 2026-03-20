using System;

namespace BankLib
{
    public class KontoPlus : Konto
    {
        private bool dostepnyLimit = true;
        private bool zablokowanePrzezDebet = false;
        private decimal limitDebetowy;

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

        public override decimal Bilans => dostepnyLimit ? bilans + limitDebetowy : 0;

        public KontoPlus(string klient, decimal bilansNaStart = 0, decimal limitDebetowy = 0)
            : base(klient, bilansNaStart)
        {
            LimitDebetowy = limitDebetowy;
        }

        public override void Wplata(decimal kwota)
        {
            if (kwota <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(kwota), "Kwota wpłaty musi być większa od zera.");
            }

            if (zablokowane && !zablokowanePrzezDebet)
            {
                throw new InvalidOperationException("Operacja niedozwolona: konto jest zablokowane z innego powodu.");
            }

            bool bylZablokowanyPrzezDebet = zablokowanePrzezDebet;

            if (bylZablokowanyPrzezDebet)
            {
                OdblokujKonto();
            }

            try
            {
                base.Wplata(kwota);
            }
            finally
            {
                if (bylZablokowanyPrzezDebet)
                {
                    if (bilans > 0)
                    {
                        zablokowanePrzezDebet = false;
                        dostepnyLimit = true;
                    }
                    else
                    {
                        BlokujKonto();
                    }
                }
            }
        }

        public override void Wyplata(decimal kwota)
        {
            if (zablokowane)
            {
                throw new InvalidOperationException("Operacja niedozwolona: konto jest zablokowane.");
            }

            if (kwota <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(kwota), "Kwota wypłaty musi być większa od zera.");
            }

            if (kwota > bilans)
            {
                if (dostepnyLimit && kwota <= bilans + LimitDebetowy)
                {
                    bilans -= kwota;
                    dostepnyLimit = false;
                    zablokowanePrzezDebet = true;
                    BlokujKonto();
                }
                else
                {
                    throw new InvalidOperationException("Brak wystarczających środków na koncie.");
                }
            }
            else
            {
                bilans -= kwota;
            }
        }
    }
}
