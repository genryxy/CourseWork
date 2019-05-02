using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MobileAppPhoto
{
    public class RecordsDataAccess
    {
        // Хранит строку подключения
        private SQLiteConnection database;
        // Объект для реализации блокировок при операциях над данными БД
        // в целях избежания конфликтов
        private static object collisionLock = new object();

        /// <summary>
        /// Позволяет известить внешние объекты о том, что коллекция была изменена.
        /// </summary>
        public ObservableCollection<Record> Records { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public RecordsDataAccess()
        {
            database = DependencyService.Get<IDatabaseConnection>().DbConnection();
            database.CreateTable<Record>();
            Records = new ObservableCollection<Record>(database.Table<Record>());
        }

        /// <summary>
        /// Количество записей
        /// </summary>
        public int CountRecords { get => Records.Count; }
       
        /// <summary>
        /// Метод для получения информации о записи
        /// </summary>
        /// <returns> список, включающий пути до фотографий и дату создания фотографии </returns>
        public List<string> GetInfoRecord()
        {
            var tempRecord = Records[Records.Count - 1];
            return new List<string>(new string[] {tempRecord.PathToImageName,
                tempRecord.PathToImageComposition, tempRecord.DateOfPhoto.ToString("hh:mm:ss_dd.MM.yyyy")
            });
        }

        /// <summary>
        /// Добавляет новый объект Record в набор Records.
        /// </summary>
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
        /// Получение экземпляра объекта по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Record GetRecord(int id)
        {
            lock (collisionLock)
            {
                return database.Table<Record>().FirstOrDefault(record => record.Id == id);
            }
        }

        #region Методы сохранения записей в БД
        /// <summary>
        /// Вставка или обновление одного экземпляра объекта Record в зависимости от наличия у него идентификатора класса Record
        /// </summary>
        /// <param name="recordInstance"></param>
        /// <returns></returns>
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
        /// Вставка или обновление всех экземпляров Record
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
        /// Удаляет указанный экземпляр из БД и списка Records
        /// </summary>
        /// <param name="recordInstance"></param>
        /// <returns></returns>
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
        /// Удаляет все объкты из таблицы
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
