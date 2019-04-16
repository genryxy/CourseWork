using System;
using System.Collections.Generic;
using System.Text;

namespace MobileAppPhoto
{
    public class ProductComposition
    {
        /// <summary>
        /// Слова, которые могу обозначать пищевую ценность. 
        /// </summary>
        private HashSet<string> composition = new HashSet<string>(new string[] { "белки",
            "белков", "белка", "жиры", "жиров", "жира", "углеводы", "углеводов", "углевода", "бел", "углево",
            "жи", "угле", "уг"});

        /// <summary>
        /// Конструктор
        /// </summary>
        public ProductComposition()
        { }

        /// <summary>
        /// Извлекает из текста данные о составе продукта
        /// </summary>
        /// <param name="detectText"> распознанный текст </param>
        /// <returns> возвращает строку со значениями белков, жиров, углеводов </returns>
        public string SearchValuesCompos(string detectText)
        {
            var words = detectText.Split(new string[] {"", "не ", " ", ":", "\t", "\n", "г,", "г.", "-", "t",
                "r", ";", "более", "менее", "больше", "меньше", "превышает", "ниже", "превышать"}, StringSplitOptions.RemoveEmptyEntries);
            string temp = string.Empty, answ = string.Empty;
            for (int i = 0; i < words.Length; i++)
            {
                if (composition.Contains(words[i]))
                {
                    /*for (int j = i + 1; j < words.Length; j++)
                    {
                        temp += words[j];
                        if (words[j][words[j].Length - 1] == ',' || words[j] == "," || j - i >= 3)
                        {
                            break;
                        }
                    }*/
                    if (words.Length - i > 1)
                    {
                        temp += words[i + 1];
                    }
                    answ += $"{words[i]}: {temp};";
                    temp = string.Empty;
                }                       
            }

            // Добавляет к результату символы для разделения, если не весь состав был записан
            while(answ.Split(':').Length != 4)
            {
                answ += ":2,4;";
            }
            return answ;
        }
    }
}
