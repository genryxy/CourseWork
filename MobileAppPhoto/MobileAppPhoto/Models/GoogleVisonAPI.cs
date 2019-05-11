using Google.Cloud.Vision.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Core;
using Grpc.Auth;
using System;
using System.Collections.Generic;
using Android.Content.Res;

namespace MobileAppPhoto
{
    /// <summary>
    /// Класс для работы Google Vision API для распознавания
    /// текста с фотографии.
    /// </summary>
    public class GoogleVisonAPI
    {
        GoogleCredential credential;
        Channel channel;        

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public GoogleVisonAPI()
        {
            GetCredentials();
        }

        /// <summary>
        /// Распознаёт текст с фотографии. 
        /// </summary>
        /// <returns> Текст, который был распознан на фотографии. </returns>
        public string DetectTextFromImage(string imageFilePath)
        {
            string msg = string.Empty;
            try
            {
                var client = ImageAnnotatorClient.Create(channel);
                var image = Image.FromFile(imageFilePath);
                IReadOnlyList<EntityAnnotation> textAnnotations = client.DetectText(image);

                foreach (EntityAnnotation text in textAnnotations)
                {
                    if (msg == string.Empty)
                    {
                        return text.Description;
                    }
                }
            }
            catch (Exception) { }
            return msg;
        }

        /// <summary>
        /// Получает необходимые для использования Google API права.
        /// </summary>
        private void GetCredentials()
        {
            try
            {
                // Получение пути до файла.
                AssetManager assets = Android.App.Application.Context.Assets;
                var stream = Android.App.Application.Context.Assets.Open("hseOcrPrivateKey.json");
                // Получение прав путём чтения json-файла с приватным ключом.
                credential = GoogleCredential.FromStream(stream);
                channel = new Channel(ImageAnnotatorClient.DefaultEndpoint.Host,
                    ImageAnnotatorClient.DefaultEndpoint.Port, credential.ToChannelCredentials());
            }
            catch (Exception) { }
        }
    }
}