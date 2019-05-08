using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MobileAppPhoto
{
    /// <summary>
    /// Класс для работы Google Vision API для распознавания
    /// текста с фотографии.
    /// </summary>
    public class MicrosoftAPI
    {
        const string subscriptionKey = "c2271daf468a4c7ca9344bd4ac282fde";
        const string uriBase = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr?";
        const string pathToResultString = "text.txt";

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public MicrosoftAPI() { }

        /// <summary>
        /// Записывает в файл текст, распознанный с указанного изображения с помощью API REST.
        /// </summary>
        /// <param name="imageFilePath"> Путь до фотографии. </param>
        private static async Task MakeOCRRequest(string imageFilePath)
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
                HttpResponseMessage response;

                byte[] byteData = GetImageAsByteArray(imageFilePath);
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    // Асинхронный вызов REST API метода.
                    response = await client.PostAsync(uri, content);
                }

                string contentString = await response.Content.ReadAsStringAsync();
                // Записывает ответ в файл в формате JSON.
                File.WriteAllText(pathToResultString, JToken.Parse(contentString).ToString());
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Читает файл с распознанным текстом (строки в формате JSON).
        /// </summary>
        /// <param name="imageFilePath"> Путь до фотографии. </param>
        /// <returns> Текст с фотографии. </returns>
        public string DetectTextFromImage(string imageFilePath)
        {
            MakeOCRRequest(imageFilePath).Wait();
            string res = string.Empty, lineText;
            try
            {
                FileStream fs = new FileStream(pathToResultString, FileMode.Open);
                using (StreamReader sr = new StreamReader(fs))
                {
                    string[] temp;
                    while (!sr.EndOfStream)
                    {
                        if ((lineText = sr.ReadLine()).Contains("text"))
                        {
                            temp = lineText.Split(new string[] { "\"", ":", " " }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < temp.Length; i++)
                            {
                                if (temp[i] == "text" && i + 1 < temp.Length)
                                {
                                    res += temp[i + 1] + " ";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
            return res;
        }

        /// <summary>
        /// Возвращает содержимое указанного файла в виде массива байтов.
        /// </summary>
        /// <param name="imageFilePath"> Путь до фотографии для чтения. </param>
        /// <returns> Двоичный массив содержимого файла. </returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
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
