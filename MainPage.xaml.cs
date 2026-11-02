using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Counter
{
    public class CounterItem : INotifyPropertyChanged
    {
        private string name;
        private int value;

        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(); }
        }

        public int Value
        {
            get => value;
            set { this.value = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }

    public partial class MainPage : ContentPage
    {
        ObservableCollection<CounterItem> counters = new();
        string filePath = Path.Combine(FileSystem.AppDataDirectory, "counters.xml");

        public MainPage()
        {
            InitializeComponent();
            LoadFromXml();
            ListView.ItemsSource = counters;
        }

        private void LoadFromXml()
        {
            if (File.Exists(filePath))
            {
                var doc = XDocument.Load(filePath);
                foreach (var el in doc.Descendants("Counter"))
                {
                    counters.Add(new CounterItem
                    {
                        Name = el.Attribute("Name")?.Value ?? "",
                        Value = int.TryParse(el.Attribute("Value")?.Value, out int v) ? v : 0
                    });
                }
            }
        }

        private void SaveToXml()
        {
            var doc = new XDocument(
                new XElement("Counters",
                    counters.Select(c => new XElement("Counter",
                        new XAttribute("Name", c.Name),
                        new XAttribute("Value", c.Value)
                    ))
                )
            );
            doc.Save(filePath);
        }

        private void Add_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameInput.Text)) return;
            int.TryParse(InitInput.Text, out int initVal);

            counters.Add(new CounterItem { Name = NameInput.Text, Value = initVal });
            SaveToXml();

            NameInput.Text = "";
            InitInput.Text = "";
        }

        private void Minus_Clicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is CounterItem c)
            {
                c.Value--;
                SaveToXml();
            }
        }

        private void Plus_Clicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is CounterItem c)
            {
                c.Value++;
                SaveToXml();
            }
        }

        private void Delete_Clicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is CounterItem c)
            {
                counters.Remove(c);
                SaveToXml();
            }
        }
    }
}