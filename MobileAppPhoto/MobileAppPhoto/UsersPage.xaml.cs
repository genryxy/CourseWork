using Plugin.Connectivity;
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
        private const string notify = "Оповещение";
        private const string warning = "Предупреждение!";
        private const string confirmation = "ОК";
        private const string internetAccess = "Необходимо подключение к интернету";
        private const string notAccessPick = "К сожалению, выбор фотографий невозможен";
        private const string notAccessCamera = "К сожалению, камера недоступна";
        private const string photoNameRequirement = "Основная часть фотографии должна включать название продукта";
        private const string photoComposRequirement = "Основная часть фотографии должна включать состав продукта";

        private RecordsDataAccess dataAccess;
        private GoogleVisonAPI googleVision;
        private ProductName productName;
        private ProductComposition productComposition;
        private MediaFile fileProdName, fileProdCompos;
        private EditPage edtPage;
        private string strProductName = "Название продукта", strProductCompos = "Состав: продукта; :;углеводы:15;";

        public UsersPage()
        {
            InitializeComponent();

            // Экземпляр RecordDataAccess Class, используемый для связывания с данными и доступа к данным
            dataAccess = new RecordsDataAccess();
            googleVision = new GoogleVisonAPI();
            productName = new ProductName();
            productComposition = new ProductComposition();

            btnTakePhoto.Clicked += BtnTakePhoto_Clicked;
            btnPickPhoto.Clicked += BtnPickPhoto_Clicked;
        }

        private async void BtnTakePhoto_Clicked(object sender, EventArgs e)
        {
            if (!CheckConnection())
            {
                await DisplayAlert(warning, internetAccess, confirmation);
                return;
            }
            string text;
            strProductName = "НазваниеПродукта";
            strProductCompos = "белки:1.5;жиры:2,7;углеводы:15;";

            // Проверка доступности камеры
            if (!CrossMedia.Current.IsCameraAvailable)
            {
                await DisplayAlert(warning, notAccessCamera, confirmation);
                return;
            }

            await DisplayAlert(notify, photoNameRequirement, confirmation);
            fileProdName = await GetMediaFileAsync();
            // Была ли сделана фотография
            if (fileProdName == null)
                return;


            googleVision.PathToImage = fileProdName.Path;
            text = googleVision.DetectTextFromImage();
            strProductName = productName.SearchWordInHashset(text);

            await DisplayAlert(notify, photoComposRequirement, confirmation);
            fileProdCompos = await GetMediaFileAsync();
            // Была ли сделана фотография
            if (fileProdCompos == null)
                return;

            googleVision.PathToImage = fileProdCompos.Path;
            text = googleVision.DetectTextFromImage();
            strProductCompos = productComposition.SearchValuesCompos(text);

            await Navigation.PushAsync(edtPage = new EditPage(strProductName, strProductCompos, OnGetPreviousPage));
            await DisplayAlert(notify, edtPage.ProdName + " " + edtPage.ProdCompos, confirmation);

            #region Альтернативный вывод фотки
            /*image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });*/
            #endregion

        }

        private async void BtnPickPhoto_Clicked(object sender, EventArgs e)
        {
            if (!CheckConnection())
            {
                await DisplayAlert(warning, internetAccess, confirmation);
                return;
            }

            string text;
            strProductName = "НазваниеПродукта";
            strProductCompos = "белки:1.5;жиры:2,7;углеводы:15;";

            // Проверка доступности выбора фотографий
            if (!CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert(warning, notAccessPick, confirmation);
                return;
            }

            await DisplayAlert(notify, photoNameRequirement, confirmation);
            fileProdName = await PickMediaFileAsync();
            // Была ли сделана фотография
            if (fileProdName == null)
                return;

            googleVision.PathToImage = fileProdName.Path;
            text = googleVision.DetectTextFromImage();
            strProductName = productName.SearchWordInHashset(text);

            await DisplayAlert(notify, photoComposRequirement, confirmation);
            fileProdCompos = await PickMediaFileAsync();
            // Была ли сделана фотография
            if (fileProdCompos == null)
                return;

            googleVision.PathToImage = fileProdCompos.Path;
            text = googleVision.DetectTextFromImage();
            strProductCompos = productComposition.SearchValuesCompos(text);

            await Navigation.PushAsync(edtPage = new EditPage(strProductName, strProductCompos, OnGetPreviousPage));
            await DisplayAlert(notify, edtPage.ProdName + " " + edtPage.ProdCompos, confirmation);

        }

        /// <summary>
        /// Открывает камеру и позволяет пользователю сфотографировать, затем сохраняет файл в указанную директорию
        /// </summary>
        /// <returns> созданная фотография </returns>
        private async Task<MediaFile> GetMediaFileAsync()
        {
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
            return file;
        }

        /// <summary>
        /// Позволяет выбрать фотографию из существующих
        /// </summary>
        /// <returns> выбранная фотография </returns>
        private async Task<MediaFile> PickMediaFileAsync()
        {
            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                PhotoSize = PhotoSize.Medium,

            });
            return file;
        }


        /// <summary>
        /// Обработчик события. Добавляет запись в БД по окончании редактирования.
        /// </summary>
        private void OnGetPreviousPage()
        {
            dataAccess.AddNewRecord(edtPage.ProdName, edtPage.ProdCompos,
                fileProdName.Path, fileProdCompos.Path);
            OnSaveClick(this, new EventArgs());
            //"path1", "path2");
        }

        /// <summary>
        /// Событие, генерируемое при выводе страницы
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Экземпляр RecordDataAccess является источником привязки к данным
            BindingContext = dataAccess;
            if (dataAccess != null && dataAccess.CountRecords > 0)
            {
                imageName.Source = ImageSource.FromFile(dataAccess.GetInfoRecord()[0]);
                imageCompos.Source = ImageSource.FromFile(dataAccess.GetInfoRecord()[1]);
                date.Text = dataAccess.GetInfoRecord()[2];
            }
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
        /// Добавляем нового пользователя в набор Records
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddClick(object sender, EventArgs e)
        {
            //dataAccess.AddNewRecord("nothing", "noText");
        }

        /// <summary>
        /// Удаляем текущую запись. Если она есть в базе данных, то будет удален и оттуда.
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

        /// <summary>
        /// Проверяет состояние подключения
        /// </summary>
        /// <returns> true - есть подключение, false - отсутствует </returns>
        private bool CheckConnection()
        {
            if (CrossConnectivity.Current != null && CrossConnectivity.Current.ConnectionTypes != null
                && CrossConnectivity.Current.IsConnected)
            {
                //var connectionType = CrossConnectivity.Current.ConnectionTypes.FirstOrDefault();                
                return true;
            }
            return false;
        }
    }
}