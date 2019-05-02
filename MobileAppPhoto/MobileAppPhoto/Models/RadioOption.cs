using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MobileAppPhoto
{
    public class RadioOption : INotifyPropertyChanged
    {
        private bool _isSelected;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="title"> Заголовок, то есть вариант ответа </param>
        /// <param name="isSelected"> Выбрана ли данная запись </param>
        /// <param name="category"> Ключ для определения принадлежности записи к конкретной группе </param>
        public RadioOption(string title, bool isSelected = false, RadioCategory category = RadioCategory.Variants)
        {
            Category = category;
            Title = title;
            IsSelected = isSelected;
        }

        /// <summary>
        /// Значение, по которому группируются записи
        /// </summary>
        public RadioCategory Category { get; private set; }
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// Выбрана ли данная запись
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if(value != _isSelected)
                {
                    _isSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //  CallerMemberName - Позволяет получить имя метода или свойства, вызывающего метод.
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;        
    }

    /// <summary>
    /// Перечисление, содержащее возможные значения ключей для сортировки
    /// </summary>
    public enum RadioCategory
    {
        Variants
    }
}
