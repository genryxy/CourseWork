using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileAppPhoto
{
    /// <summary>
    /// Класс для предоставления дополнительной информации о приложении.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoPage : ContentPage
    {
        /// <summary>
        /// Конструктор класса.
        /// </summary>
		public InfoPage()
        {
            InitializeComponent();
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
    }
}