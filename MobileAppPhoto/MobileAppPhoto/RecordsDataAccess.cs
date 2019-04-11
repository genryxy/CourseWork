using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace MobileAppPhoto
{
    public class RecordsDataAccess
    {
        // хранит строку подключения
        private SQLiteConnection database;
        // объект для реализации блокировок при операциях над данными БД
        // в целях избежания конфликтов
        private static object collisionLock = new object();

        /// <summary>
        /// Позволяет известить внешние объекты о том, что коллекция была изменена.
        /// </summary>
        public ObservableCollection<Record> Records { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public RecordsDataAccess()
        {
            database = DependencyService.Get<IDatabaseConnection>().DbConnection();
            database.CreateTable<Record>();
            Records = new ObservableCollection<Record>(database.Table<Record>());

            /*if (!database.Table<Record>().Any())
            {
                AddNewRecord("nothing", "noText");
            } */
        }

        public int CountRecords { get => Records.Count; }
       
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
        /// Фильтрация по странам с помощью LINQ-запроса
        /// </summary>
        /// <param name="countryName"> название страны </param>
        /// <returns></returns>
        public IEnumerable<Record> GetFilteredRecords(string countryName)
        {
            lock (collisionLock)
            {
                var query = from record in database.Table<Record>()
                            where record.ProductComposition == countryName
                            select record;
                return query.AsEnumerable();
            }
        }

        /*
        // Фильтрация с помощью Query
        public IEnumerable<Record> GetFilteredCustomers(string countryName)
        {
            lock (collisionLock)
            {
                return database.Query<Record>(
                  $"SELECT * FROM Item WHERE Country = '{countryName}'").
                  AsEnumerable();
            }
        }*/

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
