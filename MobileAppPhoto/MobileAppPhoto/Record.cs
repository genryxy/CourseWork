using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.ComponentModel;

namespace MobileAppPhoto
{
    [Table("Users")]
    public class Record : INotifyPropertyChanged
    {
        private int _id;
        private string _productName;
        private string _productComposition;
        private string _pathToImageComposition;
        private string _pathToImageName;
        private DateTime _date;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Id записи в таблице
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        /// <summary>
        /// Дата создания фотографии
        /// </summary>
        public DateTime DateOfPhoto
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(DateOfPhoto));
            }
        }

        /// <summary>
        /// Название продукта
        /// </summary>
        [NotNull]
        //[Column("Name")]
        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }

        /// <summary>
        /// Состав продукта
        /// </summary>
        [NotNull]
        //[Column("Composition")]
        public string ProductComposition
        {
            get => _productComposition;
            set
            {
                _productComposition = value;
                OnPropertyChanged(nameof(ProductComposition));
            }
        }
                     
        /// <summary>
        /// Путь до фотографии, на которой находится состав продукта
        /// </summary>
        public string PathToImageComposition
        {
            get => _pathToImageComposition;
            set
            {
                _pathToImageComposition = value;
                OnPropertyChanged(nameof(PathToImageComposition));
            }
        }

        /// <summary>
        /// Путь до фотографии, на которой находится название продукта 
        /// </summary>
        public string PathToImageName
        {
            get => _pathToImageName;
            set
            {
                _pathToImageName = value;
                OnPropertyChanged(nameof(PathToImageName));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
