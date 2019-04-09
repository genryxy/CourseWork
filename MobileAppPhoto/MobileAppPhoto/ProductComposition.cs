using System;
using System.Collections.Generic;
using System.Text;

namespace MobileAppPhoto
{
    public class ProductComposition
    {
        private HashSet<string> composition = new HashSet<string>(new string[] { "белки",
            "белков", "белка", "жиры", "жиров", "жира", "углеводы", "углеводов", "углевода", "бел", "углево",
            "жи", "угле", "уг"});

        public ProductComposition()
        { }

        /// <summary>
        /// Извлекает из текста данные о составе продукта
        /// </summary>
        /// <param name="detectText"> распознанный текст </param>
        /// <returns> возвращает строку со значениями белков, жиров, углеводов </returns>
        public string SearchKeyWords(string detectText)
        {
            var words = detectText.Split(new string[] { " ", ":", "\t", "\n", "г,", "г.", "-", "t", "r", ";"}, StringSplitOptions.RemoveEmptyEntries);
            string temp = string.Empty, answ = string.Empty;
            for (int i = 0; i < words.Length; i++)
            {
                if (composition.Contains(words[i]))
                {
                    for (int j = i + 1; j < words.Length; j++)
                    {
                        if (words[j][words[j].Length - 1] == ',' || words[j] == "," || j - i >= 4)
                        {
                            break;
                        }
                        temp += words[j];
                    }
                    answ += $"{words[i]}: {temp};";
                }                       
            }

            while(answ.Split(':').Length != 4)
            {
                answ += ":;";
            }
            return answ;
        }
    }
}
