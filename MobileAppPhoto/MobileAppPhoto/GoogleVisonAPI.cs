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
        /// <summary>
        /// Путь до фотографии
        /// </summary>
        public string PathToImage { get; private set; }

        public GoogleVisonAPI(string pathToImage)
        {
            PathToImage = pathToImage;
            GetCredentials();
        }

        /// <summary>
        /// Распознаёт текст с фотографии
        /// </summary>
        /// <returns> распозаннный текст </returns>
        public string DetectTextFromImage()
        {
            //var client = ImageAnnotatorClient.Create();
            var client = ImageAnnotatorClient.Create(channel);
            var image = Image.FromFile(PathToImage);
            IReadOnlyList<EntityAnnotation> textAnnotations = client.DetectText(image);
            string msg = textAnnotations.ToString();
            return msg;
        }

        /// <summary>
        /// Получает необходимые права
        /// </summary>
        private void GetCredentials()
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            var asset = Android.App.Application.Context.Assets.Open("hseOcrPrivateKey.json");

            credential = GoogleCredential.FromStream(asset);
            channel = new Channel(ImageAnnotatorClient.DefaultEndpoint.Host,
                ImageAnnotatorClient.DefaultEndpoint.Port, credential.ToChannelCredentials());
        }
    }
}



//string workingDirectory = MobileAppPhoto.Properties.Resources.hseOcrPrivateKey;
//credential = GoogleCredential.FromFile(path2);