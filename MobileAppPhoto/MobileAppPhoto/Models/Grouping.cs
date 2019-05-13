using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MobileAppPhoto
{
    /// <summary>
    /// Класс, представляющий сгруппированные элементы для списка.
    /// </summary>
    /// <typeparam name="K"> Ключ, по которому группируют. </typeparam>
    /// <typeparam name="T"> Коллекция, элементы которой имеют одинаковый ключ. </typeparam>
    public class Grouping<K, T> : ObservableCollection<T>
    {
        /// <summary>
        /// Значение, по которому группируются элементы.
        /// </summary>
        public K Key { get; private set; }
        /// <summary>
        /// Отображается ли название группы.
        /// </summary>
        public bool IsVisible { get; private set; } = true;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="key"> Значение, по которому группируются элементы. </param>
        /// <param name="items"> Коллекция, элементы которой имеют одинаковый ключ. </param>
        public Grouping(K key, IEnumerable<T> items)
        {
            Key = key;
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }
    }
}
