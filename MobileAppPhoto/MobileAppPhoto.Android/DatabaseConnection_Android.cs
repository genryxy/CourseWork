using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileAppPhoto.Droid;
using SQLite;

// Атрибут Xamarin.Forms.Dependency указывает, что данный класс реализует необходимый интерфейс.
[assembly: Xamarin.Forms.Dependency(typeof(DatabaseConnection_Android))]
namespace MobileAppPhoto.Droid
{
    public class DatabaseConnection_Android : IDatabaseConnection
    {
        /// <summary>
        /// Подключение к БД для ОС Android
        /// </summary>
        /// <returns></returns>
        public SQLiteConnection DbConnection()
        {
            var dbName = "UsersInfo.db3";
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);
            return new SQLiteConnection(path);
        }
    }
}