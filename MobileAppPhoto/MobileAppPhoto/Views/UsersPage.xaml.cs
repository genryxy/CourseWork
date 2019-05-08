using Plugin.Connectivity;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace MobileAppPhoto
{
    /// <summary>
    /// Класс главной страницы пользователя. Доступ ко всем функциям
    /// приложения осуществляется с данной страницы.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersPage : MasterDetailPage
    {
        #region Константные строки
        private const string confirmation = "Подтверждение";
        private const string cancel = "Отмена";
        private const string ok = "ОК";
        private const string fullRemoval = "Вы уверены? Данные нельзя будет восстановить.";
        private const string internetAccess = "Необходимо подключение к интернету";
        private const string notAccessPick = "К сожалению, выбор фотографий невозможен";
        private const string notAccessCamera = "К сожалению, камера недоступна";
        private const string notify = "Оповещение";
        private const string photoNameRequirement = "Основная часть фотографии должна включать название продукта";
        private const string photoComposRequirement = "Основная часть фотографии должна включать состав продукта";
        private const string warning = "Предупреждение!";
        #endregion

        private RecordsDataAccess dataAccess;
        private GoogleVisonAPI googleVision;
        private MicrosoftAPI microsoftAPI;
        private ProductName productName;
        private ProductComposition productComposition;
        private MediaFile _fileProdName, _fileProdComposition;
        private EditPage edtPage;
        private DatabasePage dbPage;
        private SettingsPage settingPage;
        private string _strProductName = "НазваниеПродукта";
        private string _strProductCompos = "_:0;_:0;_:0;";
        private string _selectedAPI = "Google API";
        private string _text = string.Empty;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public UsersPage()
        {
            InitializeComponent();
            BackgroundColor = Color.White;

            // Экземпляр RecordDataAccess, используемый для связывания с данными и доступа к данным.
            dataAccess = new RecordsDataAccess();
            googleVision = new GoogleVisonAPI();
            productName = new ProductName();
            productComposition = new ProductComposition();
            microsoftAPI = new MicrosoftAPI();
        }

        #region Полный цикл работы с фотографиями
        /// <summary>
        /// Обработчик события. Позволяет сделать 2 фотографии.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnTakePhoto_Clicked(object sender, EventArgs e)
        {
            if (!CheckConnection())
            {
                await DisplayAlert(warning, internetAccess, ok);
                return;
            }
            ResetValues();

            // Проверка доступности камеры.
            if (!CrossMedia.Current.IsCameraAvailable)
            {
                await DisplayAlert(warning, notAccessCamera, ok);
                return;
            }

            await DisplayAlert(notify, photoNameRequirement, ok);
            _fileProdName = await GetMediaFileAsync();
            // Была ли сделана фотография.
            if (_fileProdName == null)
            {
                WaitProcessingPhoto(true);
                return;
            }
            CheckSelectionStatus();
            MakeAPIRequest(ref _text, _fileProdName);
            _strProductName = productName.SearchWordInHashset(_text);

            await DisplayAlert(notify, photoComposRequirement, ok);
            _fileProdComposition = await GetMediaFileAsync();
            // Была ли сделана фотография.
            if (_fileProdComposition == null)
            {
                WaitProcessingPhoto(true);
                return;
            }
            MakeAPIRequest(ref _text, _fileProdComposition);
            _strProductCompos = productComposition.SearchValuesCompos(_text);

            await Navigation.PushAsync(edtPage = new EditPage(_strProductName, _strProductCompos, OnSaveRecord));
            WaitProcessingPhoto(true);
            await DisplayAlert(notify, edtPage.ProdName + " " + edtPage.ProdCompos, ok);
        }

        /// <summary>
        /// Обработчик события. Даёт возможность выбрать 2 фотографии из существующих.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnPickPhoto_Clicked(object sender, EventArgs e)
        {
            if (!CheckConnection())
            {
                await DisplayAlert(warning, internetAccess, ok);
                return;
            }
            ResetValues();

            // Проверка доступности выбора фотографий.
            if (!CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert(warning, notAccessPick, ok);
                return;
            }

            await DisplayAlert(notify, photoNameRequirement, ok);
            _fileProdName = await PickMediaFileAsync();
            // Была ли выбрана фотография.
            if (_fileProdName == null)
            {
                WaitProcessingPhoto(true);
                return;
            }
            CheckSelectionStatus();
            MakeAPIRequest(ref _text, _fileProdName);
            _strProductName = productName.SearchWordInHashset(_text);

            await DisplayAlert(notify, photoComposRequirement, ok);
            _fileProdComposition = await PickMediaFileAsync();
            // Была ли выбрана фотография.
            if (_fileProdComposition == null)
            {
                WaitProcessingPhoto(true);
                return;
            }
            MakeAPIRequest(ref _text, _fileProdComposition);
            _strProductCompos = productComposition.SearchValuesCompos(_text);

            await Navigation.PushAsync(edtPage = new EditPage(_strProductName, _strProductCompos, OnSaveRecord));
            WaitProcessingPhoto(true);
            await DisplayAlert(notify, edtPage.ProdName + " " + edtPage.ProdCompos, ok);
        }
        #endregion

        #region Методы для получения фотографии
        /// <summary>
        /// Открывает камеру и позволяет пользователю сфотографировать, затем сохраняет файл в указанную директорию.
        /// </summary>
        /// <returns> Созданная фотография. </returns>
        private async Task<MediaFile> GetMediaFileAsync()
        {
            WaitProcessingPhoto(false);
            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Test",
                SaveToAlbum = true,
                CompressionQuality = 75,
                CustomPhotoSize = 50,
                PhotoSize = PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 2000,
                Name = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + "_app.jpg",
                DefaultCamera = CameraDevice.Rear
            });
            return file;
        }

        /// <summary>
        /// Даёт возможность выбрать фотографию из существующих.
        /// </summary>
        /// <returns> Выбранная фотография. </returns>
        private async Task<MediaFile> PickMediaFileAsync()
        {
            WaitProcessingPhoto(false);
            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                PhotoSize = PhotoSize.Medium
            });
            return file;
        }
        #endregion        

        /// <summary>
        /// Событие, генерируемое при выводе страницы.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Экземпляр RecordDataAccess является источником привязки к данным.
            BindingContext = dataAccess;
            if (dataAccess != null && dataAccess.CountRecords > 0)
            {
                imageName.Source = ImageSource.FromFile(dataAccess.GetInfoRecord()[0]);
                imageCompos.Source = ImageSource.FromFile(dataAccess.GetInfoRecord()[1]);
                date.Text = dataAccess.GetInfoRecord()[2];
            }
            else
            {
                imageName.Source = null;
                imageCompos.Source = null;
                date.Text = null;
                InitializeComponent();
            }
        }

        #region Просмотр записей БД
        /// <summary>
        /// Посмотреть последнюю запись, которая содержится в БД.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnViewOneClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(dbPage = new DatabasePage(dataAccess, 1));
        }

        /// <summary>
        /// Посмотреть 2 последних записи, которые содержатся в БД.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnViewTwoClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(dbPage = new DatabasePage(dataAccess, 2));
        }

        /// <summary>
        /// Посмотреть 3 последних записи, которые содержатся в БД.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnViewThreeClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(dbPage = new DatabasePage(dataAccess, 3));
        }

        /// <summary>
        /// Посмотреть 4 последних записи, которые содержатся в БД.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnViewFourClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(dbPage = new DatabasePage(dataAccess, 4));
        }

        /// <summary>
        /// Посмотреть все записи, которые содержатся в БД.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnViewAll_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(dbPage = new DatabasePage(dataAccess, dataAccess.CountRecords));

        }
        #endregion

        #region Обработчики событий
        /// <summary>
        /// Обработчик события. Добавляет запись в БД по окончании редактирования.
        /// </summary>
        private void OnSaveRecord()
        {
            dataAccess.AddNewRecord(edtPage.ProdName, edtPage.ProdCompos,
                _fileProdName.Path, _fileProdComposition.Path);
            OnSaveClick(this, new EventArgs());
        }

        /// <summary>
        /// Обработчик события. Сохраняет любые отложенные изменения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveClick(object sender, EventArgs e)
        {
            dataAccess.SaveAllRecords();
        }

        /// <summary>
        /// Обработчик события. Удаляет текущую запись.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnRemoveClick(object sender, EventArgs e)
        {
            if (RecordsView.SelectedItem is Record currentRecord)
            {
                var result = await DisplayAlert(confirmation, fullRemoval, ok, cancel);
                if (result)
                {
                    dataAccess.DeleteRecord(currentRecord);
                }
            }
        }

        /// <summary>
        /// Обработчик события. Получить дополнительную информацию о приложении.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnGetInfo_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoPage());
        }

        /// <summary>
        /// Обработчик события. Открыть страницу для изменения настроек.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnChangeSettings_Clicked(object sender, EventArgs e)
        {
            CheckSelectionStatus();
            await Navigation.PushAsync(settingPage = new SettingsPage(_selectedAPI));
        }

        /// <summary>
        /// Обработчик события. Получить инструкцию по использованию приложением.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnGetHelp_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HelpPage());
        }

        /// <summary>
        /// Обработчик события. Удаляет всех пользователей. Используется 
        /// объект DisplayAlert, чтобы запросить подтверждение у пользователя.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnRemoveAll_Clicked(object sender, EventArgs e)
        {
            if (dataAccess.Records.Any())
            {
                var result = await DisplayAlert(confirmation, fullRemoval, ok, cancel);

                if (result)
                {
                    dataAccess.DeleteAllRecords();
                    BindingContext = dataAccess;
                }
            }
            OnAppearing();
        }
        #endregion

        #region Вспомогательные методы
        /// <summary>
        /// Делает соответствующий API запрос в зависимости от значения selectedAPI.
        /// </summary>
        /// <param name="text"> Текст, извлеченный с фотографии (передаётся с ref). </param>
        /// <param name="file"> Фотография. </param>
        private void MakeAPIRequest(ref string text, MediaFile file)
        {
            if (_selectedAPI == "Google API" || _selectedAPI.Length > 9)
            {
                text = googleVision.DetectTextFromImage(file.Path);
            }
            else
            {
                text = microsoftAPI.DetectTextFromImage(file.Path);
            }
        }

        /// <summary>
        /// Проверяет состояние подключения к интернету.
        /// </summary>
        /// <returns> true - есть подключение, false - отсутствует. </returns>
        private bool CheckConnection()
        {
            return (CrossConnectivity.Current != null) && (CrossConnectivity.Current.ConnectionTypes != null)
                && CrossConnectivity.Current.IsConnected;
        }

        /// <summary>
        /// Проверяет выбранный для использования API.
        /// </summary>
        private void CheckSelectionStatus()
        {
            if (settingPage != null)
            {
                _selectedAPI = settingPage.SelectedAPI;
            }
        }

        /// <summary>
        /// Устанавливает исходные значения переменных.
        /// </summary>
        private void ResetValues()
        {
            _text = string.Empty;
            _strProductName = "НазваниеПродукта";
            _strProductCompos = "_:0;_:0;_:0;";
        }

        /// <summary>
        /// Блокирует кнопки и выводит информацию во время обработки фотографии.
        /// </summary>
        /// <param name="isEnabled"> Обрабатывается ли фотография. </param>
        private void WaitProcessingPhoto(bool isEnabled)
        {
            btnTakePhoto.IsEnabled = isEnabled;
            btnPickPhoto.IsEnabled = isEnabled;
            lblWaiting.IsVisible = !isEnabled;
            ViewOne.IsEnabled = isEnabled;
            ViewTwo.IsEnabled = isEnabled;
            ViewThree.IsEnabled = isEnabled;
            ViewFour.IsEnabled = isEnabled;
            btnViewAll.IsEnabled = isEnabled;
            btnGetHelp.IsEnabled = isEnabled;
            btnGetInfo.IsEnabled = isEnabled;
            btnChangeSettings.IsEnabled = isEnabled;
            Remove.IsEnabled = isEnabled;
            btnGetInfo.IsEnabled = isEnabled;
            btnRemoveAll.IsEnabled = isEnabled;
        }
        #endregion
    }
}