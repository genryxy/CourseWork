using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileAppPhoto
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private ObservableCollection<Grouping<string, RadioOption>> radioOptions = new ObservableCollection<Grouping<string, RadioOption>>();

        public string SelectedAPI { get; private set; }
        public SettingsPage()
        {
            InitializeComponent();
            Initialize();
        }

        private void lstViewRadio_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is RadioOption item))
                return;

            // Проверка на то, что выбран только один элемент
            foreach (var group in radioOptions)
            {
                if(group.Contains(item))
                {
                    foreach (var s in group.Where(x => x.IsSelected))
                    {
                        s.IsSelected = false;
                    }
                    item.IsSelected = true;
                    SelectedAPI = item.Title;
                }
            }

        }

        /// <summary>
        /// Очищает выбранный элемент, чтобы он не подсвечивался долго
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstViewRadio_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            lstViewRadio.SelectedItem = null;
        }

        private async void btnSelect_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Оповещение", "Изменения сохранены", "ок");
            await Navigation.PopAsync();
        }

        private void Initialize()
        {
            var items = new List<RadioOption>()
            {
                new RadioOption("Google API", true),
                new RadioOption("Microsoft API")
            };

            // Копирует элементы в группы
            var sorted = from item in items
                         group item by item.Category into radioGroups
                         select new Grouping<string, RadioOption>("Варианты:", radioGroups);

            radioOptions = new ObservableCollection<Grouping<string, RadioOption>>(sorted);
            lstViewRadio.ItemsSource = radioOptions;
        }
    }
}