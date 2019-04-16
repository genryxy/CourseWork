using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileAppPhoto
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatabasePage : ContentPage
    {
        private const string back = "Вернуться на главную страницу";
        private const string approvement = "Подтверждение";
        private const string confirmation = "ОК";

        private int Top { get; set; }
        private RecordsDataAccess DataAccess { get; set; }
        private int Left { get; } = 10;
        public event Action getPreviousPage;

        public DatabasePage(RecordsDataAccess dataAccess, int countRecords, Action onGetPreviousPage)
        {
            InitializeComponent();
            DataAccess = dataAccess;
            getPreviousPage += onGetPreviousPage;
            ScrollView scrollView = new ScrollView();
            StackLayout stackLayoutEntries = new StackLayout();
            StackLayout stackLayoutAll = new StackLayout();
            ListView listView = new ListView(); // TODO можно выбирать

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
                Entry entryId = new Entry { Text = $"{records[records.Count - i - 1].Id}", IsEnabled = false };
                Entry entryDate = new Entry { Text = $"{records[records.Count - i - 1].DateOfPhoto}", IsEnabled = false };
                Entry entryName = new Entry { Text = $"{records[records.Count - i - 1].ProductName}", IsEnabled = false };
                Entry entryCompos = new Entry { Text = $"{records[records.Count - i - 1].ProductComposition}", IsEnabled = false };
                Label labelSpace = new Label { BackgroundColor = Color.DarkGray };

                stackLayoutEntries.Children.Add(entryId);
                stackLayoutEntries.Children.Add(entryDate);
                stackLayoutEntries.Children.Add(entryName);
                stackLayoutEntries.Children.Add(entryCompos);
                stackLayoutEntries.Children.Add(labelSpace);
            }
            scrollView.Content = stackLayoutEntries;
            stackLayoutAll.Children.Add(scrollView);

            Button btnReturn = new Button { Text = back, BackgroundColor = Color.Gray};
            btnReturn.Clicked += BtnReturn_Clicked;
            if (records.Count < countRecords)
            {
                stackLayoutAll.Children.Add(numberErr);
            }
            stackLayoutAll.Children.Add(btnReturn);           

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

        /// <summary>
        /// Удаляем текущую запись. Если она есть в базе данных, то будет удален и оттуда.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRemoveClick(object sender, EventArgs e)
        {
            var currentRecord = //RecordsView.SelectedItem as Record;
                new Record();
            if (currentRecord != null)
            {
                DataAccess.DeleteRecord(currentRecord);
            }
        }

        /// <summary>
        /// Удаляет всех пользователей. Используется объект DisplayAlert, чтобы запросить подтверждение у пользователя.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnRemoveAllClick(object sender, EventArgs e)
        {
            if (DataAccess.Records.Any())
            {
                var result = await DisplayAlert(approvement,
                    "Вы уверены? Данные нельзя будет восстановить.", confirmation, "Cancel");

                if (result)
                {
                    DataAccess.DeleteAllRecords();
                    BindingContext = DataAccess;
                }
            }
        }
    }
}
