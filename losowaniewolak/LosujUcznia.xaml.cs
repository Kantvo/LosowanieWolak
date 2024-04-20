using System.Collections.ObjectModel;

namespace losowaniewolak;

public partial class LosujUcznia : ContentPage
{
    public string Nazwa { get; set; }
    public ObservableCollection<Uczen> ListaUczniow = new ObservableCollection<Uczen>();

    public LosujUcznia()
    {
        InitializeComponent();

        listaUczniow.ItemsSource = ListaUczniow;
        WczytajUczniowZPliku();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        WczytajUczniowZPliku();
    }
    private string PobierzSciezkePliku()
    {
        return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Klasy"), $"{Nazwa}.txt");
    }
    private void WczytajUczniowZPliku()
    {
        string sciezkaPliku = PobierzSciezkePliku();
        if (File.Exists(sciezkaPliku))
        {
            var linie = File.ReadAllLines(sciezkaPliku);
            foreach (var linia in linie)
            {
                ListaUczniow.Add(new Uczen { Imie = linia });
            }
        }
    }

    private void ZapiszUczniowDoPliku()
    {
        string sciezkaPliku = PobierzSciezkePliku();
        File.WriteAllLines(sciezkaPliku, ListaUczniow.Select(e => e.Imie));
    }
    private async void DodajUcznia(object sender, EventArgs e)
    {
        string imie = await DisplayPromptAsync("Nowy uczeñ", "Podaj imiê ucznia", "Dodaj", "Anuluj");
        if (string.IsNullOrWhiteSpace(imie) || imie == "Anuluj")
            return;

        Uczen uczen = new Uczen { Imie = imie };
        ListaUczniow.Add(uczen); 
        ZapiszUczniowDoPliku();
    }

    private async void EdytujUcznia(object sender, EventArgs e)
    {
        if (listaUczniow.SelectedItem is Uczen wybranyUczen)
        {
            string noweImie = await DisplayPromptAsync("Edycja ucznia", "Nowe imiê ucznia", initialValue: wybranyUczen.Imie, accept: "Edytuj", cancel: "Anuluj");
            if (!string.IsNullOrWhiteSpace(noweImie) && noweImie != "Anuluj")
            {
                wybranyUczen.Imie = noweImie;
                ZapiszUczniowDoPliku();
            }
        }
    }
    private void UsunUcznia(object sender, EventArgs e)
    {
        if (listaUczniow.SelectedItem is Uczen wybranyUczen)
        {
            ListaUczniow.Remove(wybranyUczen);
            ZapiszUczniowDoPliku();
        }
    }


    private async void LosowanieUcznia(object sender, EventArgs e)
    {
        if (ListaUczniow.Count <= 0)
        {
            await DisplayAlert("B³¹d w trakcie losowania uczniów", "Klasa nie posiada ¿adnych uczniów! Dodaj uczniów.", "OK");
        }

        try
        {
            Random rand = new Random();
            int indeksLosowegoUcznia = rand.Next(0, ListaUczniow.Count);
            Uczen losowyUczen = ListaUczniow[indeksLosowegoUcznia];

            await DisplayAlert("Wylosowano Ucznia", $"Wylosowany Uczeñ: {losowyUczen.Imie}", "OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wyst¹pi³ b³¹d podczas losowania ucznia: {ex.Message}");
        }
    }
    private async void WczytajKlase(object sender, EventArgs e)
    {
        FileResult plik = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Wybierz plik .txt"
        });

        if (plik == null || Path.GetExtension(plik.FullPath) != ".txt")
        {
            await DisplayAlert("B³¹d", "Wyst¹pi³ b³¹d w trakcie wybierania pliku", "OK");
            return;
        }

        string tekst = File.ReadAllText(plik.FullPath);

        string[] linie = tekst.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        ListaUczniow.Clear(); 

        foreach (string linia in linie)
        {
            ListaUczniow.Add(new Uczen() { Imie = linia }); 
        }

        await DisplayAlert("Sukces", "Pomyœlnie wczytano listê uczniów", "OK");
    }
}