using Android.Content.Res;
using System;
using System.Collections.Generic;
using System.IO;

namespace MobileAppPhoto
{
    /// <summary>
    /// Класс для работы с текстом для определения названия продукта.
    /// </summary>
    public class ProductName
    {
        // При загрузке файла в свойствах поменять "Действие при сборке" на "Andoid asset".
        private readonly AssetManager assets = Android.App.Application.Context.Assets;
        private readonly Stream stream = Android.App.Application.Context.Assets.Open("productName.txt");

        /// <summary>
        /// Множество, содержащее названия продуктов.
        /// </summary>
        private HashSet<string> AllName { get; set; } = new HashSet<string>();

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public ProductName()
        {
            //CreateHashset();
        }

        /// <summary>
        /// Проверяет наличие слова в множестве, хранящем названия продуктов.
        /// Возвращает текст с фотографии в нужном формате.
        /// </summary>
        /// <param name="detectText"> Распознанный текст. </param>
        /// <returns> Название продукта. </returns>
        public string SearchWordInHashset(string detectText)
        {
            if (detectText != null)
            {
                string[] arrWords = detectText.Split(new string[] { " ", ":", ".", "\n", "\t" },
                    StringSplitOptions.RemoveEmptyEntries);
                string res = string.Empty;
                // Вывод текста без проверки наличия в множестве с названиями продуктов.
                foreach (var word in arrWords)
                {
                    res += word.ToLower().Trim() + " ";
                }
                if (res == " ")
                {
                    res = "_";
                }

                // Проверка наличия названия продукта в множестве с названиями продуктов.
                /*foreach (var word in arrWords)
                {
                    if (AllName.Contains(word.ToLower().Trim()))
                    {
                        return word.ToLower().Trim();
                    }
                }
                return "_";*/
                return res;
            }
            return "_";
        }

        /// <summary>
        /// Заполняет множество словами из текстового файла.
        /// </summary>
        private void CreateHashset()
        {
            string word;
            try
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    while (!sr.EndOfStream)
                    {
                        word = sr.ReadLine();
                        AllName.Add(word.Trim().ToLower());
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
