using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileAppPhoto
{
    /// <summary>
    /// Класс для просмотра содержмого БД.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatabasePage : ContentPage
    {
        const string back = "Вернуться на главную страницу";
        const string errorText = "Было выведено меньшее количество записей, так как в базе " +
                "данных хранится меньше записей.";
        const string textColor = "#1B4F72";
        const string backColor = "#BDC3C7";

        /// <summary>
        /// Ссылка на экзмепляр класса RecordsDataAccess для работы с БД.
        /// </summary>
        private RecordsDataAccess DataAccess { get; set; }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="dataAccess"> Ссылка на экзмепляр класса RecordsDataAccess для работы с БД. </param>
        /// <param name="countRecords"> Количество выводимых записей. </param>
        public DatabasePage(RecordsDataAccess dataAccess, int countRecords)
        {
            InitializeComponent();
            BackgroundColor = Color.White;
            DataAccess = dataAccess;
            ScrollView scrollView = new ScrollView();
            StackLayout stackLayoutEntries = new StackLayout();
            StackLayout stackLayoutAll = new StackLayout();

            Label numberErr = new Label
            {
                Text = errorText,
                TextColor = Color.Red
            };

            var records = dataAccess.Records;
            var bound = Math.Min(records.Count, countRecords);
            for (int i = 0; i < bound; i++)
            {
                Label lblId = new Label
                {
                    Text = $"{records[records.Count - i - 1].Id}",
                    FontSize = 20,
                    TextColor = Color.Black
                };
                Label lblDate = new Label
                {
                    Text = $"Дата: {records[records.Count - i - 1].DateOfPhoto}",
                    FontSize = 20,
                    TextColor = Color.Black
                };
                Label lblName = new Label
                {
                    Text = $"Название: {records[records.Count - i - 1].ProductName}",
                    FontSize = 20,
                    TextColor = Color.Black
                };
                Label lblCompos = new Label
                {
                    Text = $"Состав: {records[records.Count - i - 1].ProductComposition}",
                    FontSize = 20,
                    TextColor = Color.Black
                };
                Label lblSpace = new Label { BackgroundColor = Color.LightGray };

                stackLayoutEntries.Children.Add(lblId);
                stackLayoutEntries.Children.Add(lblDate);
                stackLayoutEntries.Children.Add(lblName);
                stackLayoutEntries.Children.Add(lblCompos);
                stackLayoutEntries.Children.Add(lblSpace);
            }
            scrollView.Content = stackLayoutEntries;
            stackLayoutAll.Children.Add(scrollView);

            Button btnReturn = new Button
            {
                Text = back,
                BackgroundColor = Color.FromHex(backColor),
                TextColor = Color.FromHex(textColor)
            };
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
            await Navigation.PopAsync();
        }
    }
}
