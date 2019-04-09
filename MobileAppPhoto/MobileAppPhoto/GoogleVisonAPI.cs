using Google.Cloud.Vision.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Core;
using Grpc.Auth;
using System;
using System.Collections.Generic;
using Android.Content.Res;
using Grpc;

namespace MobileAppPhoto
{
    public class GoogleVisonAPI
    {
        GoogleCredential credential;
        Channel channel;

        //public event Func<string> CheckTextLanguage;

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
            var client = ImageAnnotatorClient.Create(channel);
            var image = Image.FromFile(PathToImage);
            IReadOnlyList<EntityAnnotation> textAnnotations = client.DetectText(image);            
            string msg = string.Empty; 
           
            foreach (EntityAnnotation text in textAnnotations)
            {
                if (msg == string.Empty)
                {
                    // Проверям язык текста с фотографии
                    //if (text.Locale == "ru")
                    //{
                        return text.Description;
                    //}
                    //msg = CheckTextLanguage?.Invoke();
                }
                break;
            }
            return msg;
        }

        /// <summary>
        /// Получает необходимые права
        /// </summary>
        private void GetCredentials()
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            var stream = Android.App.Application.Context.Assets.Open("hseOcrPrivateKey.json");
            credential = GoogleCredential.FromStream(stream);
            channel = new Channel(ImageAnnotatorClient.DefaultEndpoint.Host,
                ImageAnnotatorClient.DefaultEndpoint.Port, credential.ToChannelCredentials());
        }
    }
}