using SQLite;

namespace MobileAppPhoto
{
    /// <summary>
    /// Интерфейс для подключения к БД
    /// </summary>
    public interface IDatabaseConnection
    {
        SQLiteConnection DbConnection();
    }
}
