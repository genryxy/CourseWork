﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace MobileAppPhoto
{
    /// <summary>
    /// Объект, представляющий сгруппированные элементы для списка
    /// </summary>
    /// <typeparam name="K"> Ключ, по которому группируют </typeparam>
    /// <typeparam name="T"> Коллекция, элементы которой имеют одинаковый ключ </typeparam>
    public class Grouping<K, T> : ObservableCollection<T>
    {
        /// <summary>
        /// Значение, по которому группируются элементы
        /// </summary>
        public K Key { get; private set; }
        /// <summary>
        /// Отображается ли название группы
        /// </summary>
        public bool IsVisible { get; set; } = false;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="key"> Значение, по которому группируются элементы </param>
        /// <param name="items"> Коллекция, элементы которой имеют одинаковый ключ </param>
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