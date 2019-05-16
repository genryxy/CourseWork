using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MobileAppPhoto
{
    /// <summary>
    /// Класс для работы Microsoft Computer Vision API для распознавания
    /// текста с фотографии.
    /// </summary>
    public class MicrosoftAPI
    {
        const string subscriptionKey = "c2271daf468a4c7ca9344bd4ac282fde";
        const string uriBase = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr?";

        /// <summary>
        /// Ответ, записанный в формате JSON.
        /// </summary>
        private static string DetectedText { get; set; }
        /// <summary>
        /// Текст, распознанный на фотографии.
        /// </summary>
        public static string ResultText { get; private set; }

        /// <summary>
        /// Ответ, получаемый при вызове API.
        /// </summary>
        private static HttpResponseMessage Response { get; set; }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public MicrosoftAPI() { }

        /// <summary>
        /// Записывает в свойство DetectedText текст, распознанный с помощью API REST
        /// с указанного изображения .
        /// </summary>
        /// <param name="imageFilePath"> Путь до фотографии. </param>
        public static async Task MakeOCRRequest(string imageFilePath)
        {
            try
            {
                HttpClient client = new HttpClient();
                // Заголовок запроса.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                // Параметры запроса. Язык определяется автоматически. 
                // detectOrientation=true, поэтому корректирует ориентацию текста перед его распознаванием.
                string requestParameters = "language=unk&detectOrientation=true";
                // Создание запроса.
                string uri = uriBase + requestParameters;


                byte[] byteData = GetImageAsByteArray(imageFilePath);
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    // Асинхронный вызов REST API метода.
                    Response = await client.PostAsync(uri, content).ConfigureAwait(false); ;
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Сохраняет распознанные значения. Вызывается из класса UserPage.
        /// </summary>
        public async static void OnDetectTextMicrosoft()
        {
            DetectedText = await Response.Content.ReadAsStringAsync();
            GetWordsFromHttpResponse();
        }

        /// <summary>
        /// Вытаскивает слова из JSON ответа и записывает их в переменную. 
        /// </summary>
        private static void GetWordsFromHttpResponse()
        {
            string res = string.Empty;
            string[] wordsInLine;
            string[] allLines = DetectedText.Split('\n');
            foreach (var lineText in allLines)
            {
                if (lineText.Contains("text"))
                {
                    wordsInLine = lineText.Split(new string[] { "\"", ":", " " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < wordsInLine.Length; i++)
                    {
                        if (wordsInLine[i] == "text" && i + 1 < wordsInLine.Length)
                        {
                            res += wordsInLine[i + 1] + " ";
                        }
                    }
                }
            }
            ResultText = res;
        }

        /// <summary>
        /// Возвращает содержимое указанного файла в виде массива байтов.
        /// </summary>
        /// <param name="imageFilePath"> Путь до фотографии для чтения. </param>
        /// <returns> Массив байтов содержимого файла. </returns>
        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            try
            {
                using (FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader binaryReader = new BinaryReader(fileStream);
                    return binaryReader.ReadBytes((int)fileStream.Length);
                }
            }
            catch (Exception) { }
            return null;
        }
    }
}
