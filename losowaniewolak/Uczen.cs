using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace losowaniewolak
{
    public class Uczen : INotifyPropertyChanged
    {
        private string _imie;
        public string Imie
        {
            get => _imie;
            set
            {
                if (_imie != value)
                {
                    _imie = value;
                    OnPropertyChanged(nameof(Imie));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
