using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Microsoft.Maui.Controls;

namespace losowaniewolak
{
    public partial class MainPage : ContentPage
    {
        private string sciezkaFolderuKlas = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Klasy");
        public ObservableCollection<LosujUcznia> Klasy { get; set; }

        public MainPage()
        {
            InitializeComponent();
            Klasy = new ObservableCollection<LosujUcznia>();
            klasy.BindingContext = this;

            if (!Directory.Exists(sciezkaFolderuKlas))
                Directory.CreateDirectory(sciezkaFolderuKlas);

            Debug.WriteLine(sciezkaFolderuKlas);

            WczytajKlasy();
        }

        private async void DodajKlase(object sender, EventArgs e)
        {
            string nazwa = await DisplayPromptAsync("Tworzenie", "Podaj nazwę nowej klasy", "Stwórz", "Anuluj");
            if (string.IsNullOrWhiteSpace(nazwa) || nazwa == "Anuluj")
                return;

            LosujUcznia nowaKlasa = new LosujUcznia()
            {
                Nazwa = nazwa
            };

            ZapiszKolekcjeDoPliku(nowaKlasa);
            Klasy.Add(nowaKlasa);
        }

        private async void WybierzKlase(object sender, EventArgs e)
        {
            if (!(sender is Button button) || !(button.CommandParameter is LosujUcznia wybranaKlasa))
                return;

            await Navigation.PushAsync(new LosujUcznia() { Nazwa = wybranaKlasa.Nazwa });
        }

        private async void UsunKlase(object sender, EventArgs e)
        {
            if (!(sender is Button button) || !(button.CommandParameter is LosujUcznia wybranaKlasa))
                return;

            bool potwierdzonoUsuniecie = await DisplayAlert("Potwierdzenie", $"Czy na pewno chcesz usunąć klasę {wybranaKlasa.Nazwa}?", "Tak", "Anuluj");
            if (!potwierdzonoUsuniecie)
                return;

            UsunPlikKlasy(wybranaKlasa);
            Klasy.Remove(wybranaKlasa);
        }

        private void WczytajKlasy()
        {
            var pliki = Directory.GetFiles(sciezkaFolderuKlas);

            foreach (string plik in pliki)
            {
                var nazwaPliku = Path.GetFileName(plik).Split(".")[0];

                LosujUcznia klasa = new LosujUcznia() { Nazwa = nazwaPliku };

                Klasy.Add(klasa);
            }
        }

        private void ZapiszKolekcjeDoPliku(LosujUcznia klasa)
        {
            string sciezkaPliku = Path.Combine(sciezkaFolderuKlas, $"{klasa.Nazwa}.txt");
            try
            {
                File.Create(sciezkaPliku).Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w trackie zapisywania pliku: {ex.Message}");
            }
        }

        private void UsunPlikKlasy(LosujUcznia klasa)
        {
            string sciezkaPliku = Path.Combine(sciezkaFolderuKlas, $"{klasa.Nazwa}.txt");
            if (File.Exists(sciezkaPliku))
            {
                try
                {
                    File.Delete(sciezkaPliku);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd w trakcie usuwania pliku: {ex.Message}");
                }
            }
        }
    }
}