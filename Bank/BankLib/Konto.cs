namespace BankLib
{
    public class Konto
    {
        protected string klient;  
        internal decimal bilans;  
        protected bool zablokowane = false;

        public Konto(string klient, decimal bilansNaStart = 0)
        {
            this.klient = klient;
            this.bilans = bilansNaStart;
        }

        public string Nazwa => klient;
        public virtual decimal Bilans => bilans;
        public bool Zablokowane => zablokowane;

        public virtual void Wplata(decimal kwota)
        {
            if (zablokowane)
            {
                throw new System.InvalidOperationException("Operacja niedozwolona: konto jest zablokowane.");
            }

            if (kwota <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(kwota), "Kwota wpłaty musi być większa od zera.");
            }

            bilans += kwota;
        }

        public virtual void Wyplata(decimal kwota)
        {
            if (zablokowane)
            {
                throw new System.InvalidOperationException("Operacja niedozwolona: konto jest zablokowane.");
            }

            if (kwota <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(kwota), "Kwota wypłaty musi być większa od zera.");
            }

            if (kwota > bilans)
            {
                throw new System.InvalidOperationException("Brak wystarczających środków na koncie.");
            }

            bilans -= kwota;
        }

        public void BlokujKonto()
        {
            zablokowane = true;
        }

        public void OdblokujKonto()
        {
            zablokowane = false;
        }
    }
}
