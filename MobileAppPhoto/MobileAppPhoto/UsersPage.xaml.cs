using Plugin.Media;
using Plugin.Media.Abstractions;
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
    public partial class UsersPage : ContentPage
    {
        private UsersDataAccess dataAccess;
        private delegate void ChangePageHandler();
        private event ChangePageHandler ChangePage;

        public UsersPage()
        {
            InitializeComponent();

            // Экземпляр UserDataAccessClass, используемый для связывания с данными и доступа к данным
            dataAccess = new UsersDataAccess();

            // Привязать событие к нажатию кнопки для фотографирования
            btnTakePhoto.Clicked += BtnTakePhoto_Clicked;
        }

        private async void BtnTakePhoto_Clicked(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Test",
                SaveToAlbum = true,
                CompressionQuality = 75,
                CustomPhotoSize = 50,
                PhotoSize = PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 2000,
                Name = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + "_app.jpg",
                DefaultCamera = CameraDevice.Front
            });

            if (file == null)
                return;
            var googleVision = new GoogleVisonAPI(file.Path);
            await DisplayAlert("File Location", file.Path, "OK");

            //image.Source = ImageSource.FromFile(file.Path);

            dataAccess.AddNewUser(file.Path, googleVision.DetectTextFromImage());
            image.Source = ImageSource.FromFile(dataAccess[0]);

            //ChangePage += new App().OnChangePage;
            //ChangePage?.Invoke();

            /*image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });*/
        }

        /// <summary>
        /// Событие, генерируемое при выводе страницы
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Экземпляр UserDataAccess является источником привязки к данным
            BindingContext = dataAccess;
        }

        /// <summary>
        /// Сохраняем любые отложенные изменения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveClick(object sender, EventArgs e)
        {
            dataAccess.SaveAllUSers();
        }

        /// <summary>
        /// Добавляем нового пользователя в набор Users
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddClick(object sender, EventArgs e)
        {
            dataAccess.AddNewUser("nothing", "noText");
        }

        /// <summary>
        /// Удаляем текущего пользователя. Если он есть в базе данных, то будет удален и оттуда.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRemoveClick(object sender, EventArgs e)
        {
            var currentUser = UsersView.SelectedItem as User;
            if (currentUser != null)
            {
                dataAccess.DeleteUser(currentUser);
            }
        }

        /// <summary>
        /// Удаляет всех пользователей. Используется объект DisplayAlert, чтобы запросить подтверждение у пользователя.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnRemoveAllClick(object sender, EventArgs e)
        {
            if (dataAccess.Users.Any())
            {
                var result = await DisplayAlert("Подтверждение",
                    "Вы уверены? Данные нельзя будет восстановить.", "Ok", "Cancel");

                if (result)
                {
                    dataAccess.DeleteAllUsers();
                    BindingContext = dataAccess;
                }
            }
        }
    }
}