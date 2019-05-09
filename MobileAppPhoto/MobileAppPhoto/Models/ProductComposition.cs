using System;
using System.Collections.Generic;

namespace MobileAppPhoto
{
    /// <summary>
    /// Класс для работы с текстом для определения состава продукта.
    /// </summary>
    public class ProductComposition
    {
        /// <summary>
        /// Слова, которые могут обозначать пищевую ценность. 
        /// </summary>
        private HashSet<string> composition = new HashSet<string>(new string[] { "белки", "белков", "белок", "жир",
            "белка", "жиры", "жиров", "жира", "углеводы", "углеводов", "углевода", "бел", "углево", "угле", "улеводы",
            "еводы", "еводов", "евод", "леводы", "леводов", "левод", "елка", "елков", "елок", "елко"});
        private readonly string[] _splitWords = {"не", ".", "•", " ", ":", "\t", "\n", "г,", "г.", "-", "—", "t", "r", ";",
                 "более", "менее", "больше", "меньше", "превышает", "ниже", "превышать", "щенные жир", "может"};

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public ProductComposition() { }

        /// <summary>
        /// Извлекает из текста данные о составе продукта.
        /// </summary>
        /// <param name="detectText"> Распознанный текст с фотографии. </param>
        /// <returns> Строка со значениями белков, жиров, углеводов. </returns>
        public string SearchValuesCompos(string detectText)
        {
            string answ = string.Empty;
            if (detectText != null)
            {
                var words = detectText.Split(_splitWords, StringSplitOptions.RemoveEmptyEntries);
                string temp = string.Empty;
                for (int i = 0; i < words.Length; i++)
                {
                    if (composition.Contains(words[i].ToLower()))
                    {
                        if (words.Length - i > 1)
                        {
                            temp += words[i + 1];
                        }
                        answ += $"{words[i].ToLower().Trim()}: {temp};";
                        temp = string.Empty;
                    }
                }
            }
            // Добавляет к результату символы для разделения, если не все значения были записаны.
            while (answ.Split(':').Length < 4)
            {
                answ += "_:0;";
            }
            return answ;
        }
    }
}
