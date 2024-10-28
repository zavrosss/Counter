using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Microsoft.Maui.Controls;

namespace Testing
{
    public partial class MainPage : ContentPage
    {
        // Lista przechowująca liczniki
        private readonly List<Counter> _counters;

        public MainPage()
        {
            InitializeComponent();
            _counters = new List<Counter>();
            LoadCounterData(); // Ładowanie istniejących liczników podczas inicjalizacji
        }

        // Obsługa kliknięcia przycisku "Dodaj licznik"
        private void OnAddCounterClicked(object sender, EventArgs e)
        {
            // Pobieranie i czyszczenie białych znaków z nazwy licznika oraz wartości początkowej
            var name = CounterNameEntry.Text?.Trim();
            var initialValueStr = InitialValueEntry.Text?.Trim();
            var initialValue = int.TryParse(initialValueStr, out var parsedValue) ? parsedValue : 0;

            // Sprawdzenie, czy pole nazwy jest puste //DODATKOWA!
            if (string.IsNullOrWhiteSpace(name))
            {
                DisplayAlert("Błąd", "Nazwa licznika nie może być pusta.", "OK");
                return;
            }

            // Sprawdzenie, czy licznik o tej samej nazwie już istnieje //DODATKOWA!
            if (IsCounterNameDuplicate(name))
            {
                DisplayAlert("Błąd", $"Licznik o nazwie \"{name}\" już istnieje!", "OK");
                return;
            }

            // Dodanie nowego licznika z unikalną nazwą
            var newCounter = new Counter(name) { Value = initialValue };
            _counters.Add(newCounter);
            PersistCounterData(); // Zapisanie stanu liczników
            RefreshCounterDisplay(); // Aktualizacja widoku
            ResetInputFields(); // Czyszczenie pól wejściowych
        }

        // Sprawdza, czy licznik o danej nazwie już istnieje na liście
        private bool IsCounterNameDuplicate(string name)
        {
            // Sprawdzenie nazw ignorując wielkość liter
            return _counters.Exists(counter => counter.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        // Obsługa kliknięcia przycisku "Usuń wszystkie liczniki" //DODATKOWA!
        private void OnDeleteAllCountersClicked(object sender, EventArgs e)
        {
            _counters.Clear(); // Usunięcie wszystkich liczników
            PersistCounterData(); // Zapisanie pustej listy
            RefreshCounterDisplay(); // Aktualizacja widoku
        }

        // Aktualizuje wyświetlany widok liczników
        private void RefreshCounterDisplay()
        {
            CounterContainer.Children.Clear(); // Czyszczenie kontenera liczników

            // Dodanie każdego licznika z listy do kontenera
            foreach (var counter in _counters)
            {
                var layout = CreateCounterLayout(counter); // Utworzenie układu dla licznika
                CounterContainer.Children.Add(layout); // Dodanie układu do widoku
            }
        }

        // Tworzy układ StackLayout dla licznika
        private StackLayout CreateCounterLayout(Counter counter)
        {
            var layout = new StackLayout { Orientation = StackOrientation.Horizontal };

            // Label dla nazwy licznika
            var nameLabel = new Label { Text = counter.Name, VerticalOptions = LayoutOptions.Center };

            // Label dla wartości licznika
            var valueLabel = new Label { Text = counter.Value.ToString(), VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) };

            // Przycisk inkrementacji wartości licznika
            var incrementButton = CreateCounterButton("+", () => UpdateCounterValue(counter, 1));

            // Przycisk dekrementacji wartości licznika
            var decrementButton = CreateCounterButton("-", () => UpdateCounterValue(counter, -1));

            // Dodanie elementów do układu licznika
            layout.Children.Add(nameLabel);
            layout.Children.Add(valueLabel);
            layout.Children.Add(incrementButton);
            layout.Children.Add(decrementButton);

            return layout; // Zwraca skonstruowany układ licznika
        }

        // Tworzy przycisk do aktualizacji wartości licznika
        private Button CreateCounterButton(string text, Action onClick)
        {
            var button = new Button { Text = text, WidthRequest = 30 };
            button.Clicked += (s, e) => onClick(); // Dodanie zdarzenia kliknięcia
            return button;
        }

        // Aktualizuje wartość licznika i zapisuje zmiany
        private void UpdateCounterValue(Counter counter, int change)
        {
            counter.Value += change;
            PersistCounterData(); // Zapisanie zaktualizowanego stanu
            RefreshCounterDisplay(); // Aktualizacja widoku
        }

        // Ładuje istniejące liczniki z pliku
        private void LoadCounterData()
        {
            var path = GetCounterFilePath();
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var loadedCounters = JsonConvert.DeserializeObject<List<Counter>>(json) ?? new List<Counter>();
                _counters.AddRange(loadedCounters); // Dodanie załadowanych liczników do listy
                RefreshCounterDisplay(); // Aktualizacja wyświetlania załadowanych liczników
            }
        }

        // Zapisuje aktualną listę liczników do pliku
        private void PersistCounterData()
        {
            var path = GetCounterFilePath();
            var json = JsonConvert.SerializeObject(_counters);
            File.WriteAllText(path, json); // Zapisuje stan liczników jako plik JSON
        }

        // Zwraca ścieżkę do pliku przechowującego stan liczników
        private string GetCounterFilePath() =>
            Path.Combine(FileSystem.AppDataDirectory, "counters.json");

        // Czyści pola wejściowe w interfejsie
        private void ResetInputFields()
        {
            CounterNameEntry.Text = string.Empty;
            InitialValueEntry.Text = string.Empty;
        }
    }
}
