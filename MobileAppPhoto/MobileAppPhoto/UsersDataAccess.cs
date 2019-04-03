using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace MobileAppPhoto
{
    public class UsersDataAccess
    {
        // хранит строку подключения
        private SQLiteConnection database;
        // объект для реализации блокировок при операциях над данными БД
        // в целях избежания конфликтов
        private static object collisionLock = new object();

        /// <summary>
        /// Позволяет известить внешние объекты о том, что коллекция была изменена.
        /// </summary>
        public ObservableCollection<User> Users { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public UsersDataAccess()
        {
            database = DependencyService.Get<IDatabaseConnection>().DbConnection();
            database.CreateTable<User>();
            Users = new ObservableCollection<User>(database.Table<User>());

            if (!database.Table<User>().Any())
            {
                AddNewUser("nothing", "noText");
            }
        }

        public int CountUsers { get => Users.Count; }
        public string this[int index] { get => Users[Users.Count - 1].PhysicalAddress; }

        /// <summary>
        /// Добавляет новый объект User в набор Users. Предотвращает связывание с пустым набором.
        /// </summary>
        public void AddNewUser(string filePath, string detectText)
        {
            Users.Add(new User
            {
                Country = "Russia",
                CompanyName = "compName",
                PhysicalAddress = $"{filePath}"
            });
        }

        /// <summary>
        /// Фильтрация по странам с помощью LINQ-запроса
        /// </summary>
        /// <param name="countryName"> название страны </param>
        /// <returns></returns>
        public IEnumerable<User> GetFilteredUsers(string countryName)
        {
            lock (collisionLock)
            {
                var query = from user in database.Table<User>()
                            where user.Country == countryName
                            select user;
                return query.AsEnumerable();
            }
        }

        /*
        // Фильтрация с помощью Query
        public IEnumerable<User> GetFilteredCustomers(string countryName)
        {
            lock (collisionLock)
            {
                return database.Query<User>(
                  $"SELECT * FROM Item WHERE Country = '{countryName}'").
                  AsEnumerable();
            }
        }*/

        /// <summary>
        /// Получение экземпляра объекта по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetUser(int id)
        {
            lock (collisionLock)
            {
                return database.Table<User>().FirstOrDefault(user => user.Id == id);
            }
        }

        #region Методы сохранения записей в БД
        /// <summary>
        /// Вставка или обновление одного экземпляра объекта User в зависимости от наличия у него идентификатора класса User
        /// </summary>
        /// <param name="userInstance"></param>
        /// <returns></returns>
        public int SaveUser(User userInstance)
        {
            lock (collisionLock)
            {
                if (userInstance.Id != 0)
                {
                    database.Update(userInstance);
                }
                else
                {
                    database.Insert(userInstance);
                }
                return userInstance.Id;
            }
        }

        /// <summary>
        /// Вставка или обновление всех экземпляров User
        /// </summary>
        public void SaveAllUSers()
        {
            lock (collisionLock)
            {
                foreach (var userInstance in Users)
                {
                    if(userInstance.Id != 0)
                    {
                        database.Update(userInstance);
                    }
                    else
                    {
                        database.Insert(userInstance);
                    }
                }
            }
        }
        #endregion

        #region Методы удаления записей из БД
        /// <summary>
        /// Удаляет указанный экземпляр из БД и списка Users
        /// </summary>
        /// <param name="userInstance"></param>
        /// <returns></returns>
        public int DeleteUser(User userInstance)
        {
            var id = userInstance.Id;
            if(id != 0)
            {
                lock (collisionLock)
                {
                    database.Delete<User>(id);
                }
            }
            Users.Remove(userInstance);
            return id;
        }

        /// <summary>
        /// Удаляет все объкты из таблицы
        /// </summary>
        public void DeleteAllUsers()
        {
            lock (collisionLock)
            {
                database.DropTable<User>();
                database.CreateTable<User>();
            }
            Users = null;
            Users = new ObservableCollection<User>(database.Table<User>());
        }
        #endregion
    }
}
