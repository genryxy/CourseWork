using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MobileAppPhoto
{
    /// <summary>
    /// Класс для представления варианта ответа при выборе API.
    /// </summary>
    public class RadioOption : INotifyPropertyChanged
    {
        private bool _isSelected;

        /// <summary>
        /// Событие для уведомления об изменении свойств класса.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="title"> Вариант ответа. </param>
        /// <param name="isSelected"> Выбран ли данный вариант. </param>
        /// <param name="category"> Ключ для определения принадлежности записи к конкретной группе. </param>
        public RadioOption(string title, bool isSelected = false, RadioCategory category = RadioCategory.API)
        {
            Category = category;
            Title = title;
            IsSelected = isSelected;
        }

        /// <summary>
        /// Значение, по которому группируются записи.
        /// </summary>
        public RadioCategory Category { get; private set; }
        /// <summary>
        /// Заголовок (текст, который будет выведен в качестве варианта ответа).
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// Выбрана ли данная запись.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if(value != _isSelected)
                {
                    _isSelected = value;
                    NotifyPropertyChanged(nameof(IsSelected));
                }
            }
        }

        /// <summary>
        /// Вызвает событие при изменении значения свойства.
        /// </summary>
        /// <param name="propertyName"> Имя изменяемого свойства. </param>
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    /// <summary>
    /// Перечисление, содержащее возможные значения ключей для сортировки по группам.
    /// </summary>
    public enum RadioCategory
    {
        /// <summary>
        /// Группа для представления вариантов API.
        /// </summary>
        API
    }
}
