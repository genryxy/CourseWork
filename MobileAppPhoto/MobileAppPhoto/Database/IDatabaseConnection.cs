using SQLite;

namespace MobileAppPhoto
{
    /// <summary>
    /// Интерфейс для подключения к БД (позволяет получать путь до БД
    /// с использованием платформо-зависимого кода).
    /// </summary>
    public interface IDatabaseConnection
    {
        /// <summary>
        /// Метод для получения пути до БД.
        /// </summary>
        /// <returns> Соединение с БД. </returns>
        SQLiteConnection DbConnection();
    }
}
