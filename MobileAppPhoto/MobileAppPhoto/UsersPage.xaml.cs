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
        private RecordsDataAccess dataAccess;
        private GoogleVisonAPI googleVision;
        private ProductName productName;
        private ProductComposition productComposition;
        private delegate void ChangePageHandler();
        private event ChangePageHandler ChangePage;

        public UsersPage()
        {
            InitializeComponent();

            // Экземпляр UserDataAccessClass, используемый для связывания с данными и доступа к данным
            dataAccess = new RecordsDataAccess();
            googleVision = new GoogleVisonAPI();
            productName = new ProductName();
            productComposition = new ProductComposition();

            googleVision.CheckTextLanguage += OnCheckTextLanguage;
            btnTakePhoto.Clicked += BtnTakePhoto_Clicked;
        }

        private async void BtnTakePhoto_Clicked(object sender, EventArgs e)
        {
            string strProductName = "Название продукта", strProductCompos = "Состав продукта", text;

            // Проверка наличия камеры
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "Unfortunately, no camera available.", "OK");
                return;
            }

            await DisplayAlert("Оповещение", "Сделайте фотографию так, чтобы её большая часть содержала " +
                "название продукта", "ОК");
            // Процесс фотографирования с последующим сохранением с указанными параметрами
            var fileProdName = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
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

            // Была ли сделана фотография
            if (fileProdName == null)
            {
                return;
            }
            googleVision.PathToImage = fileProdName.Path;

            await DisplayAlert("File Location", fileProdName.Path, "OK");

            text = googleVision.DetectTextFromImage();
            if (text == OnCheckTextLanguage())
            {
                await DisplayAlert("Оповещение", OnCheckTextLanguage(), "OK");
            }
            else
            {
                //  Найдено ли слово, явлвяющееся названием продукта, в распознанном тексте
                if ((strProductName = productName.SearchWordInHashset(text)) == null)
                {
                    // TODO предложить ввести текст (возможно вызвать событие)
                    // добавить в множество названий введенное название
                }
                else
                {
                    // TODO вывести распознанное слово
                    // дать возможность исправить
                }
            }
            //image.Source = ImageSource.FromFile(file.Path);
            image.Source = ImageSource.FromFile(dataAccess[0]);


            await DisplayAlert("Оповещение", "Сделайте фотографию так, чтобы на ней" +
                " был виден состава продукта", "ОК");
            // Процесс фотографирования с последующим сохранением с указанными параметрами
            var fileProdCompos = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
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
            if (fileProdCompos == null)
            {
                return;
            }
            googleVision.PathToImage = fileProdCompos.Path;

            text = googleVision.DetectTextFromImage();
            if (text == OnCheckTextLanguage())
            {
                await DisplayAlert("Оповещение", OnCheckTextLanguage(), "OK");
            }
            else
            {
                strProductCompos = productComposition.SearchKeyWords(text);
                //  Распознавание состава продукта
                if (productComposition.IsNeedEdit)
                {
                    // TODO предложить ввести текст (возможно вызвать событие)
                    // добавить в множество названий введенное название
                }
                else
                {
                    // TODO вывести распознанное слово
                    // дать возможность исправить
                }
            }

            dataAccess.AddNewRecord("allText, пока резерв", strProductName, strProductCompos,
                fileProdName.Path, fileProdCompos.Path);
            //dataAccess.AddNewRecord(file.Path, googleVision.DetectTextFromImage().ToString());

            //ChangePage += new App().OnChangePage;
            //ChangePage?.Invoke();

            #region Альтернативный вывод фотки
            /*image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });*/
            #endregion
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

        private string OnCheckTextLanguage()
        {
            return "Текст на фотографии должен быть на русском языке.";
        }

        /// <summary>
        /// Сохраняем любые отложенные изменения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveClick(object sender, EventArgs e)
        {
            dataAccess.SaveAllRecords();
        }

        /// <summary>
        /// Добавляем нового пользователя в набор Users
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddClick(object sender, EventArgs e)
        {
            //dataAccess.AddNewRecord("nothing", "noText");
        }

        /// <summary>
        /// Удаляем текущую запись. Если он есть в базе данных, то будет удален и оттуда.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRemoveClick(object sender, EventArgs e)
        {
            var currentRecord = RecordsView.SelectedItem as Record;
            if (currentRecord != null)
            {
                dataAccess.DeleteRecord(currentRecord);
            }
        }

        /// <summary>
        /// Удаляет всех пользователей. Используется объект DisplayAlert, чтобы запросить подтверждение у пользователя.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnRemoveAllClick(object sender, EventArgs e)
        {
            if (dataAccess.Records.Any())
            {
                var result = await DisplayAlert("Подтверждение",
                    "Вы уверены? Данные нельзя будет восстановить.", "Ok", "Cancel");

                if (result)
                {
                    dataAccess.DeleteAllRecords();
                    BindingContext = dataAccess;
                }
            }
        }
    }
}