using System;
using System.Collections.Generic;
using System.Text;

namespace MobileAppPhoto
{
    public class ProductComposition
    {
        public bool IsNeedEdit { get; private set; } = false;
        private HashSet<string> proteins = new HashSet<string>(new string[] { "белки", "белков", "белка" });
        private HashSet<string> fats = new HashSet<string>(new string[] { "жиры", "жиров", "жира" });
        private HashSet<string> carbohydrates = new HashSet<string>(new string[] { "углеводы", "углеводов", "углевода" });

        public ProductComposition()
        { }

        /// <summary>
        /// Извлекает из текста данные о составе продукта
        /// </summary>
        /// <param name="detectText"> распознанный текст </param>
        /// <returns> возвращает строку со значениями белков, жиров, углеводов </returns>
        public string SearchKeyWords(string detectText)
        {
            var words = detectText.Split(new string[] { " ", ":", "\t", "\n"}, StringSplitOptions.RemoveEmptyEntries);
            string temp = string.Empty, answ = string.Empty;
            for (int i = 0; i < words.Length; i++)
            {
                if (proteins.Contains(words[i]))
                {
                    for (int j = i + 1; j < words.Length; j++)
                    {
                        if (words[j][words[i].Length - 1] == ',' || words[j] == "," || j - i >= 4)
                        {
                            break;
                        }
                        temp += words[j];
                    }
                    answ += $"{words[i]}: {temp};";
                }       
                
                if (answ.Split(':').Length != 4)
                {
                    IsNeedEdit = true;
                }
            }            
            return answ;
        }
    }
}
