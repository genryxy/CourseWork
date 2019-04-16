using Android.Content.Res;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MobileAppPhoto
{
    public class ProductName
    {
        // Не забыть при загрузке файла в свойствах поменять "Действие при сборке" на "Andoid asset"
        private AssetManager assets = Android.App.Application.Context.Assets;
        private Stream stream = Android.App.Application.Context.Assets.Open("productName.txt");

        /// <summary>
        /// Множество, содержащее названия продуктов
        /// </summary>
        public HashSet<string> AllName { get; set; } = new HashSet<string>();

        /// <summary>
        /// Конструктор
        /// </summary>
        public ProductName()
        {
            CreateHashset();
        }

        /// <summary>
        /// Проверяет наличие слова в множестве, хранящем названия продуктов
        /// </summary>
        /// <param name="detectText"> проверяемое слово </param>
        /// <returns> null - отсутствует, иначе - найденное слово </returns>
        public string SearchWordInHashset(string detectText)
        {
            string[] arrWords = detectText.Split(new string[] { " ", ":", ".", "\n", "\t"},
                StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in arrWords)
            {
                if (AllName.Contains(word.ToLower().Trim()))
                {
                    return word.ToLower().Trim();
                }
            }
            return "название";
        }

        /// <summary>
        /// Заполняет множество словами из текстового файла
        /// </summary>
        private void CreateHashset()
        {
            string word;
            try
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    while ((word = sr.ReadLine()) != null)
                    {
                        AllName.Add(word.Trim().ToLower());
                    }
                }
            }
            catch(Exception e) { }
        }

    }
}
