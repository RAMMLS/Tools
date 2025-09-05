using MySql.Data.MySqlClient;
using System;

namespace MysqlUserFootprint
{
    public class Database
    {
        private string connectionString;
        private MySqlConnection connection;

        public Database(string host, string user, string password, string database)
        {
            connectionString = $"Server={host};Database={database};Uid={user};Pwd={password};";
        }

        public void Connect()
        {
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                Console.WriteLine("Успешное подключение к базе данных.");
            }
            catch (MySqlException ex)
            {
                Logger.LogError("Ошибка подключения к базе данных: " + ex.Message);
                throw; // Пробрасываем исключение выше
            }
        }

        public void LogUserActivity(UserActivity activity)
        {
            try
            {
                string query = "INSERT INTO user_activity (timestamp, user, action, details) VALUES (@timestamp, @user, @action, @details)";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@timestamp", activity.Timestamp);
                    command.Parameters.AddWithValue("@user", activity.User);
                    command.Parameters.AddWithValue("@action", activity.Action);
                    command.Parameters.AddWithValue("@details", activity.Details);

                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                Logger.LogError("Ошибка записи активности пользователя: " + ex.Message);
                throw;
            }
        }

        // Другие методы для работы с базой данных...

        public void Disconnect()
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
                Console.WriteLine("Подключение к базе данных закрыто.");
            }
        }
    }
}

