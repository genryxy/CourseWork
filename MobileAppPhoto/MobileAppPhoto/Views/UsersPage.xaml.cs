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
        private ProductName productName;
        private ProductComposition productComposition;
        private MediaFile fileProdName, fileProdCompos;
        private EditPage edtPage;
        private DatabasePage dbPage;
        private SettingsPage settingPage;
        private string strProductName = "НазваниеПродукта", strProductCompos = "_:0;_:0;_:0;";
        private string selectedAPI = "Google API";

        public UsersPage()
        {
            InitializeComponent();
            BackgroundColor = Color.White;

            // Экземпляр RecordDataAccess Class, используемый для связывания с данными и доступа к данным
            dataAccess = new RecordsDataAccess();
            googleVision = new GoogleVisonAPI();
            productName = new ProductName();
            productComposition = new ProductComposition();
        }

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
            string text;
            strProductName = "НазваниеПродукта";
            strProductCompos = ":;:;:;";

            // Проверка доступности камеры
            if (!CrossMedia.Current.IsCameraAvailable)
            {
                await DisplayAlert(warning, notAccessCamera, ok);
                return;
            }

            await DisplayAlert(notify, photoNameRequirement, ok);
            fileProdName = await GetMediaFileAsync();
            // Была ли сделана фотография
            if (fileProdName == null)
            {
                WaitProcessingPhoto(true);
                return;
            }

            WaitProcessingPhoto(false);
            googleVision.PathToImage = fileProdName.Path;
            text = googleVision.DetectTextFromImage();
            strProductName = productName.SearchWordInHashset(text);

            await DisplayAlert(notify, photoComposRequirement, ok);
            fileProdCompos = await GetMediaFileAsync();
            // Была ли сделана фотография
            if (fileProdCompos == null)
            {
                WaitProcessingPhoto(true);
                return;
            }

            googleVision.PathToImage = fileProdCompos.Path;
            text = googleVision.DetectTextFromImage();
            strProductCompos = productComposition.SearchValuesCompos(text);

            await Navigation.PushAsync(edtPage = new EditPage(strProductName, strProductCompos, OnSaveRecord));
            WaitProcessingPhoto(true);
            await DisplayAlert(notify, edtPage.ProdName + " " + edtPage.ProdCompos, ok);

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

            string text;
            strProductName = "НазваниеПродукта";
            strProductCompos = ":;:;:;";

            // Проверка доступности выбора фотографий
            if (!CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert(warning, notAccessPick, ok);
                return;
            }

            await DisplayAlert(notify, photoNameRequirement, ok);
            fileProdName = await PickMediaFileAsync();
            // Была ли выбрана фотография
            if (fileProdName == null)
            {
                WaitProcessingPhoto(true);
                return;
            }

            googleVision.PathToImage = fileProdName.Path;
            text = googleVision.DetectTextFromImage();
            strProductName = productName.SearchWordInHashset(text);
            
            await DisplayAlert(notify, photoComposRequirement, ok);
            fileProdCompos = await PickMediaFileAsync();
            // Была ли выбрана фотография
            if (fileProdCompos == null)
            {
                WaitProcessingPhoto(true);
                return;
            }

            googleVision.PathToImage = fileProdCompos.Path;
            text = googleVision.DetectTextFromImage();
            strProductCompos = productComposition.SearchValuesCompos(text);
            
            await Navigation.PushAsync(edtPage = new EditPage(strProductName, strProductCompos, OnSaveRecord));
            WaitProcessingPhoto(true);
            await DisplayAlert(notify, edtPage.ProdName + " " + edtPage.ProdCompos, ok);
        }

        /// <summary>
        /// Открывает камеру и позволяет пользователю сфотографировать, затем сохраняет файл в указанную директорию
        /// </summary>
        /// <returns> созданная фотография </returns>
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
        /// Даёт возможность выбрать фотографию из существующих
        /// </summary>
        /// <returns> выбранная фотография </returns>
        private async Task<MediaFile> PickMediaFileAsync()
        {
            WaitProcessingPhoto(false);
            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                PhotoSize = PhotoSize.Medium
            });
            return file;
        }


        /// <summary>
        /// Обработчик события. Добавляет запись в БД по окончании редактирования.
        /// </summary>
        private void OnSaveRecord()
        {
            dataAccess.AddNewRecord(edtPage.ProdName, edtPage.ProdCompos,
                fileProdName.Path, fileProdCompos.Path);
            OnSaveClick(this, new EventArgs());
        }

        private void OnViewRecords()
        {
            // do sth
        }

        /// <summary>
        /// Блокирует кнопки и выводит информацию во время обработки фотографии.
        /// </summary>
        /// <param name="isEnabled"> обрабатывается ли фотография </param>
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
            else
            {
                imageName.Source = null;
                imageCompos.Source = null;
                date.Text = null;
                InitializeComponent();
            }
        }

        #region Просмотр записей БД
        private async void OnViewOneClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(dbPage = new DatabasePage(dataAccess, 1, OnViewRecords));
        }

        private async void OnViewTwoClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(dbPage = new DatabasePage(dataAccess, 2, OnViewRecords));
        }

        private async void OnViewThreeClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(dbPage = new DatabasePage(dataAccess, 3, OnViewRecords));
        }

        private async void OnViewFourClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(dbPage = new DatabasePage(dataAccess, 4, OnViewRecords));
        }

        #endregion

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
        /// Удаляем текущую запись. Если она есть в базе данных, то будет удален и оттуда.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnRemoveClick(object sender, EventArgs e)
        {
            await DisplayAlert("check", settingPage?.SelectedAPI,"ok");
            var currentRecord = RecordsView.SelectedItem as Record;
            if (currentRecord != null)
            {
                var result = await DisplayAlert(confirmation, fullRemoval, ok, cancel);
                if (result)
                {
                    dataAccess.DeleteRecord(currentRecord);
                }
            }
        }

        /// <summary>
        /// Получить дополнительную информацию о приложении
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnGetInfo_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoPage());
        }

        /// <summary>
        /// Открыть страницу для изменения настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnChangeSettings_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(settingPage = new SettingsPage());
        }

        /// <summary>
        /// Посмотреть все записи, которые содержатся в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnViewAll_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(dbPage = new DatabasePage(dataAccess, dataAccess.CountRecords, OnViewRecords));

        }

        /// <summary>
        /// Получить инструкцию по использованию приложением
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnGetHelp_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HelpPage());
        }

        /// <summary>
        /// Удаляет всех пользователей. Используется объект DisplayAlert, чтобы запросить подтверждение у пользователя.
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


        /// <summary>
        /// Проверяет состояние подключения к интернету.
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