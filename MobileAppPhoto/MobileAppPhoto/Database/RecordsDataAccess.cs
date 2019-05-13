using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MobileAppPhoto
{
    /// <summary>
    /// Класс для доступа к данным из БД.
    /// </summary>
    public class RecordsDataAccess
    {
        // Хранит строку подключения к БД.
        private SQLiteConnection database;
        // Объект для реализации блокировок при операциях над данными БД
        // в целях избежания конфликтов.
        private static readonly object collisionLock = new object();

        /// <summary>
        /// Коллекция записей, хранящихся в БД. Тип ObservableCollection 
        /// позволяет известить внешние объекты о том, что коллекция была изменена.
        /// </summary>
        public ObservableCollection<Record> Records { get; private set; }

        /// <summary>
        /// Количество записей.
        /// </summary>
        public int CountRecords { get => Records.Count; } 

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public RecordsDataAccess()
        {
            database = DependencyService.Get<IDatabaseConnection>().DbConnection();
            database.CreateTable<Record>();
            Records = new ObservableCollection<Record>(database.Table<Record>());
        }
       
        /// <summary>
        /// Метод для получения информации о записи.
        /// </summary>
        /// <returns> Список, включающий пути до фотографий и дату создания фотографий. </returns>
        public List<string> GetInfoRecord()
        {
            var tempRecord = Records[Records.Count - 1];
            return new List<string>(new string[] {tempRecord.PathToImageName,
                tempRecord.PathToImageComposition, tempRecord.DateOfPhoto.ToString("hh:mm:ss_dd.MM.yyyy")
            });
        }

        /// <summary>
        /// Добавляет новый объект Record в коллекцию Records.
        /// </summary>
        /// <param name="name"> Название проудукта. </param>
        /// <param name="composition"> Состав продукта. </param>
        /// <param name="pathToName"> Путь до фотографии с названием. </param>
        /// <param name="pathToComposition"> Путь до фотографии с составом. </param>
        public void AddNewRecord(string name, string composition, string pathToName, string pathToComposition)
        {
            Records.Add(new Record
            {
                DateOfPhoto = DateTime.Now,
                ProductName = name,
                ProductComposition = composition,
                PathToImageName = pathToName,
                PathToImageComposition = pathToComposition                
            });
        }

        /// <summary>
        /// Получение экземпляра объекта по id.
        /// </summary>
        /// <param name="id"> Номер записи, которую необходимо получить из БД. </param>
        /// <returns> Запись из БД по указанному id. </returns>
        public Record GetRecord(int id)
        {
            lock (collisionLock)
            {
                return database.Table<Record>().FirstOrDefault(record => record.Id == id);
            }
        }

        #region Методы для сохранения записей в БД
        /// <summary>
        /// Вставка или обновление одного экземпляра объекта Record в зависимости 
        /// от наличия у него идентификатора класса Record (Id).
        /// </summary>
        /// <param name="recordInstance"> Экземпляр класса Record. </param>
        /// <returns> Номер ряда, в который записан экземпляр класса Record. </returns>
        public int SaveRecord(Record recordInstance)
        {
            lock (collisionLock)
            {
                if (recordInstance.Id != 0)
                {
                    database.Update(recordInstance);
                }
                else
                {
                    database.Insert(recordInstance);
                }
                return recordInstance.Id;
            }
        }

        /// <summary>
        /// Вставка или обновление всех экземпляров Record.
        /// </summary>
        public void SaveAllRecords()
        {
            lock (collisionLock)
            {
                foreach (var recordInstance in Records)
                {
                    if(recordInstance.Id != 0)
                    {
                        database.Update(recordInstance);
                    }
                    else
                    {
                        database.Insert(recordInstance);
                    }
                }
            }
        }
        #endregion

        #region Методы удаления записей из БД
        /// <summary>
        /// Удаляет указанный экземпляр из БД и списка Records.
        /// </summary>
        /// <param name="recordInstance"> Удаляемый экземпляр класса Record. </param>
        /// <returns> Номер строки, с которой была удалена запись. </returns>
        public int DeleteRecord(Record recordInstance)
        {
            var id = recordInstance.Id;
            if(id != 0)
            {
                lock (collisionLock)
                {
                    database.Delete<Record>(id);
                }
            }
            Records.Remove(recordInstance);
            return id;
        }

        /// <summary>
        /// Удаляет все объекты из таблицы.
        /// </summary>
        public void DeleteAllRecords()
        {
            lock (collisionLock)
            {
                database.DropTable<Record>();
                database.CreateTable<Record>();
            }
            Records = null;
            Records = new ObservableCollection<Record>(database.Table<Record>());
        }
        #endregion
    }
}
