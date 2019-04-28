using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileAppPhoto
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPage : ContentPage
    {
        Entry _nameEntry, _proteinsEntry, _fatsEntry, _carbsEntry;
        Label _currStatus;

        public string ProdName {get; private set;}
        public string ProdCompos { get; private set; }

        public event Action getPreviousPage;
        public EditPage (string prodName, string prodCompos, Action onGetPreviousPage)
		{
			InitializeComponent ();

            BackgroundColor = Color.White;
            getPreviousPage += onGetPreviousPage;
            ProdName = prodName;
            ProdCompos = prodCompos;
            ScrollView scrollView = new ScrollView();
            StackLayout stackLayout = new StackLayout();
            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = 50 },
                    new RowDefinition { Height = 60 },
                    new RowDefinition { Height = 50 },
                    new RowDefinition { Height = 60 },
                    new RowDefinition { Height = 50 },
                    new RowDefinition { Height = 60 },
                    new RowDefinition { Height = 50 },
                    new RowDefinition { Height = 60 }
                }
            };

            Label nameHeader = new Label { Text = "Название продукта", BackgroundColor = Color.LightGray };
            Label proteinsHeader = new Label { Text = "Белки", BackgroundColor = Color.LightGray };
            Label fatsHeader = new Label { Text = "Жиры", BackgroundColor = Color.LightGray };
            Label carbsHeader = new Label { Text = "Углеводы", BackgroundColor = Color.LightGray };
            _currStatus = new Label { Text = "Значения белков, жиров и углеводов должны быть числом",
                TextColor = Color.Red, IsVisible = false, FontSize = 20 };

            string[] composValues = prodCompos.Split(new char[] {';', ':'});
            _nameEntry = new Entry { Text = prodName};
            _proteinsEntry = new Entry { Text = composValues[1], Placeholder = "белки" };
            _fatsEntry = new Entry { Text = composValues[3], Placeholder = "жиры" };
            _carbsEntry = new Entry { Text = composValues[5], Placeholder = "углеводы" };
            Button btnConfirmEdit = new Button { Text = "Сохранить изменения", BackgroundColor = Color.Gray};
            btnConfirmEdit.Clicked += BtnConfirmEdit_Clicked;

            grid.Children.Add(nameHeader, 0, 0);
            grid.Children.Add(proteinsHeader, 0, 2);
            grid.Children.Add(fatsHeader, 0, 4);
            grid.Children.Add(carbsHeader, 0, 6);
            grid.Children.Add(_nameEntry, 0, 1);
            grid.Children.Add(_proteinsEntry, 0, 3);
            grid.Children.Add(_fatsEntry, 0, 5);
            grid.Children.Add(_carbsEntry, 0, 7);
                        
            stackLayout.Children.Add(grid);
            stackLayout.Children.Add(btnConfirmEdit);
            stackLayout.Children.Add(_currStatus);

            scrollView.Content = stackLayout;
            Content = scrollView;            
        }

        /// <summary>
        /// Обработчик события. При получении подтверждении на сохранение проверяет введенные
        /// данные. Если всё корректно, то изменения сохраняются.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnConfirmEdit_Clicked(object sender, EventArgs e)
        {
            if(DoubleNotNullEntryCheck(_proteinsEntry, e) || DoubleNotNullEntryCheck(_fatsEntry, e) ||
                DoubleNotNullEntryCheck(_carbsEntry, e))
            {
                _currStatus.IsVisible = true;
            }
            else
            {
                ProdName = _nameEntry.Text;
                ProdCompos = $"белки:{_proteinsEntry.Text}; жиры:{_fatsEntry.Text}; углеводы:{_carbsEntry.Text};";
                getPreviousPage?.Invoke();
                await Navigation.PopAsync();
            }
        }

        /// <summary>
        /// Проверяет ячейку на число и не пустоту
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns> true - введено число, false - иная последовательность </returns>
        private bool DoubleNotNullEntryCheck(object sender, EventArgs e)
        {
            var currentEntry = (Entry)sender;

            return !double.TryParse(currentEntry.Text.Trim().Replace(',', '.'), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out double _);               
        }
    }
}