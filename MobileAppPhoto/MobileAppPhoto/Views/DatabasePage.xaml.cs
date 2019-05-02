﻿using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileAppPhoto
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatabasePage : ContentPage
    {
        private const string back = "Вернуться на главную страницу";        
        private const string confirmation = "ОК";

        private int Top { get; set; }
        private RecordsDataAccess DataAccess { get; set; }
        private int Left { get; } = 10;
        public event Action getPreviousPage;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dataAccess"> ссылка на экзмепляр класса RecordsDataAccess для работы с БД </param>
        /// <param name="countRecords"> количество выводимых записей </param>
        /// <param name="onGetPreviousPage"> ссылка на метод, выполняемый при закрытии страницы </param>
        public DatabasePage(RecordsDataAccess dataAccess, int countRecords, Action onGetPreviousPage)
        {
            InitializeComponent();
            BackgroundColor = Color.White;
            DataAccess = dataAccess;
            getPreviousPage += onGetPreviousPage;
            ScrollView scrollView = new ScrollView();
            StackLayout stackLayoutEntries = new StackLayout();
            StackLayout stackLayoutAll = new StackLayout();
            
            Label numberErr = new Label
            {
                Text = "Было выведено меньшее количество записей, так как в базе " +
                "данных хранится меньше записей.",
                TextColor = Color.Red
            };

            var records = dataAccess.Records;
            var bound = Math.Min(records.Count, countRecords);
            for (int i = 0; i < bound; i++)
            {
                Label lblId = new Label { Text = $"{records[records.Count - i - 1].Id}", FontSize = 20, TextColor = Color.Black };
                Label lblDate = new Label { Text = $"Дата: {records[records.Count - i - 1].DateOfPhoto}", FontSize = 20, TextColor = Color.Black };
                Label lblName = new Label { Text = $"Название: {records[records.Count - i - 1].ProductName}", FontSize = 20, TextColor = Color.Black };
                Label lblCompos = new Label { Text = $"Состав: {records[records.Count - i - 1].ProductComposition}", FontSize = 20, TextColor = Color.Black };
                Label lblSpace = new Label { BackgroundColor = Color.LightGray };

                stackLayoutEntries.Children.Add(lblId);
                stackLayoutEntries.Children.Add(lblDate);
                stackLayoutEntries.Children.Add(lblName);
                stackLayoutEntries.Children.Add(lblCompos);
                stackLayoutEntries.Children.Add(lblSpace);
            }
            scrollView.Content = stackLayoutEntries;
            stackLayoutAll.Children.Add(scrollView);

            Button btnReturn = new Button { Text = back, BackgroundColor = Color.FromHex("#BDC3C7"), TextColor = Color.FromHex("#1B4F72") };
            btnReturn.Clicked += BtnReturn_Clicked;
            if (records.Count < countRecords)
            {
                stackLayoutAll.Children.Add(numberErr);
            }
            stackLayoutAll.Children.Add(btnReturn);
            stackLayoutAll.Padding = 7;
           
            Content = stackLayoutAll;
        }

        /// <summary>
        /// Обработчик события. Возвращает на предыдущую страницу.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnReturn_Clicked(object sender, EventArgs e)
        {
            getPreviousPage?.Invoke();
            await Navigation.PopAsync();
        }       
    }
}
