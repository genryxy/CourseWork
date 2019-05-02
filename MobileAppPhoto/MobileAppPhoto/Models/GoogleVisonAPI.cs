using Google.Cloud.Vision.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Core;
using Grpc.Auth;
using System;
using System.Collections.Generic;
using Android.Content.Res;

namespace MobileAppPhoto
{
    public class GoogleVisonAPI
    {
        GoogleCredential credential;
        Channel channel;
        
        /// <summary>
        /// Путь до фотографии
        /// </summary>
        public string PathToImage { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public GoogleVisonAPI()
        {
            GetCredentials();
        }

        /// <summary>
        /// Распознаёт текст с фотографии
        /// </summary>
        /// <returns> распознанный текст </returns>
        public string DetectTextFromImage()
        {
            string msg = string.Empty;
            try
            {
                var client = ImageAnnotatorClient.Create(channel);
                var image = Image.FromFile(PathToImage);
                IReadOnlyList<EntityAnnotation> textAnnotations = client.DetectText(image);

                foreach (EntityAnnotation text in textAnnotations)
                {
                    if (msg == string.Empty)
                    {
                        return text.Description;
                    }
                    break;
                }
            }
            catch (Exception) { }
            return msg;
        }

        /// <summary>
        /// Получает необходимые права
        /// </summary>
        private void GetCredentials()
        {
            try
            {
                // Получение пути до файла
                AssetManager assets = Android.App.Application.Context.Assets;
                var stream = Android.App.Application.Context.Assets.Open("hseOcrPrivateKey.json");
                // Получение прав путём чтения json-файла с приватным ключом
                credential = GoogleCredential.FromStream(stream);
                channel = new Channel(ImageAnnotatorClient.DefaultEndpoint.Host,
                    ImageAnnotatorClient.DefaultEndpoint.Port, credential.ToChannelCredentials());
            }
            catch (Exception) { }
        }
    }
}