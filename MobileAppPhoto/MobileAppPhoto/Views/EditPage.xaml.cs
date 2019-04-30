using System;
using System.Globalization;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileAppPhoto
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPage : ContentPage
    {
        Entry _nameEntry, _proteinsEntry, _fatsEntry, _carbsEntry;
        Label _currStatus;
        string[] compositonValues, correctOrderValues = new string[3]; 

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

            Label nameHeader = new Label { Text = "Название продукта", BackgroundColor = Color.LightGray, TextColor = Color.FromHex("#1B4F72")};
            Label proteinsHeader = new Label { Text = "Белки", BackgroundColor = Color.LightGray, TextColor = Color.FromHex("#1B4F72") };
            Label fatsHeader = new Label { Text = "Жиры", BackgroundColor = Color.LightGray, TextColor = Color.FromHex("#1B4F72") };
            Label carbsHeader = new Label { Text = "Углеводы", BackgroundColor = Color.LightGray, TextColor = Color.FromHex("#1B4F72") };
            _currStatus = new Label { Text = "Значения белков, жиров и углеводов должны быть числом",
                TextColor = Color.Red, IsVisible = false, FontSize = 20 };

            compositonValues = prodCompos.Split(new string[] {";", ":"}, StringSplitOptions.RemoveEmptyEntries);
            WriteValuesInCorrectOrder(compositonValues, correctOrderValues);
            _nameEntry = new Entry { Text = prodName, TextColor = Color.Black };
            _proteinsEntry = new Entry { Text = correctOrderValues[0], Placeholder = "белки", TextColor = Color.Black };
            _fatsEntry = new Entry { Text = correctOrderValues[1], Placeholder = "жиры", TextColor = Color.Black };
            _carbsEntry = new Entry { Text = correctOrderValues[2], Placeholder = "углеводы", TextColor = Color.Black };
            Button btnConfirmEdit = new Button { Text = "Сохранить изменения", BackgroundColor = Color.FromHex("#BDC3C7"), TextColor = Color.FromHex("#1B4F72")};
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
        /// Записывает значения пищевой ценности в следующем порядке: белки, жиры, углеводы.
        /// </summary>
        /// <param name="compositionValues">исходный массив</param>
        /// <param name="correctOrderValues">массив значений в правильном порядке</param>
        private void WriteValuesInCorrectOrder(string[] compositionValues, string[] correctOrderValues)
        {
            for (int i = 0; i < compositionValues.Length % 7; i += 2)
            {
                string str = compositionValues[i].ToLower().Trim();
                if(str.Contains("бел"))
                {
                    correctOrderValues[0] = compositionValues[i + 1];
                }
                else if (str.Contains("жи"))
                {
                    correctOrderValues[1] = compositionValues[i + 1];
                }
                else if(str != "_")
                {
                    correctOrderValues[2] = compositionValues[i + 1];
                }
                else
                {
                    correctOrderValues[i / 2] = compositionValues[i + 1]; 
                }
            }
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