using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileAppPhoto
{
    /// <summary>
    /// Класс для изменения настроек приложения.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        /// <summary>
        /// Коллекция возможных вариантов ответа.
        /// </summary>
        private ObservableCollection<Grouping<string, RadioOption>> radioOptions =
            new ObservableCollection<Grouping<string, RadioOption>>();

        /// <summary>
        /// Выбранное при помощи RadioButton значение.
        /// </summary>
        public string SelectedAPI { get; private set; } = "Google API";

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="selectedAPI"> Название выбранного API. </param>
        public SettingsPage(string selectedAPI)
        {
            InitializeComponent();
            SelectedAPI = selectedAPI;
            Initialize();
        }

        /// <summary>
        /// Вызывается при нажатии на вариант ответа. Меняет значение свойства IsSelected у элементов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstViewRadio_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is RadioOption item))
                return;

            // Проверка на то, что выбран только один элемент.
            foreach (var group in radioOptions)
            {
                if (group.Contains(item))
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
        /// Очищает выбранный элемент, чтобы он не подсвечивался долго.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstViewRadio_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            lstViewRadio.SelectedItem = null;
        }

        /// <summary>
        /// Обработчик события. Возвращает на главную страницу.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnReturn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Составляет список вариантов ответа для выбора API.
        /// </summary>
        private void Initialize()
        {
            var isSelected = SelectedAPI == "Google API";
            var items = new List<RadioOption>()
            {
                new RadioOption("Google API", isSelected),
                new RadioOption("Microsoft API", !isSelected)
            };

            // Копирует элементы в группы.
            var sorted = from item in items
                         group item by item.Category into radioGroups
                         select new Grouping<string, RadioOption>("Варианты:", radioGroups);

            radioOptions = new ObservableCollection<Grouping<string, RadioOption>>(sorted);
            lstViewRadio.ItemsSource = radioOptions;
        }
    }
}