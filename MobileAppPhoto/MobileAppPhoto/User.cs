using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.ComponentModel;

namespace MobileAppPhoto
{
    [Table("Users")]
    public class User : INotifyPropertyChanged
    {
        private int _id;
        private string _companyName;
        private string _country;
        private string _physicalAddress;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Id пользователя
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
        /// Имя компании
        /// </summary>
        [NotNull]
        public string CompanyName
        {
            get => _companyName;
            set
            {
                _companyName = value;
                OnPropertyChanged(nameof(CompanyName));
            }
        }

        /// <summary>
        /// Название страны
        /// </summary>
        [MaxLength(50)]
        public string Country
        {
            get => _country;
            set
            {
                _country = value;
                OnPropertyChanged(nameof(Country));
            }
        }

        // меняет название столбца
        //[Column("Address")]
        public string PhysicalAddress
        {
            get => _physicalAddress;
            set
            {
                _physicalAddress = value;
                OnPropertyChanged(nameof(PhysicalAddress));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
