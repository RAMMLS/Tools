using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SQLiTrainingApp.Models
{
    public class Database
    {
        private readonly string _dataPath;
        private List<User> _users;

        public Database()
        {
            _dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "database.json");
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (File.Exists(_dataPath))
            {
                var json = File.ReadAllText(_dataPath);
                _users = JsonConvert.DeserializeObject<List<User>>(json);
            }
            else
            {
                // Создаем тестовые данные
                _users = new List<User>
                {
                    new User { Id = 1, Username = "admin", Password = "admin123", Email = "admin@example.com", Role = "Administrator" },
                    new User { Id = 2, Username = "user1", Password = "password1", Email = "user1@example.com", Role = "User" },
                    new User { Id = 3, Username = "user2", Password = "password2", Email = "user2@example.com", Role = "User" }
                };
                SaveChanges();
            }
        }

        public void SaveChanges()
        {
            var json = JsonConvert.SerializeObject(_users, Formatting.Indented);
            File.WriteAllText(_dataPath, json);
        }

        // Уязвимый метод - подвержен SQL-инъекциям
        public User VulnerableGetUser(string username, string password)
        {
            // Имитация SQL-запроса с уязвимостью
            return _users.FirstOrDefault(u => 
                u.Username == username && u.Password == password);
        }

        // Безопасный метод - с параметризацией
        public User SafeGetUser(string username, string password)
        {
            // Имитация параметризованного запроса
            return _users.FirstOrDefault(u => 
                u.Username == username && u.Password == password);
        }

        // Метод для демонстрации UNION-атак
        public List<User> VulnerableSearchUsers(string searchTerm)
        {
            // Имитация уязвимого поиска
            return _users.Where(u => 
                u.Username.Contains(searchTerm) || 
                u.Email.Contains(searchTerm) || 
                u.Role.Contains(searchTerm)).ToList();
        }

        public List<User> GetAllUsers()
        {
            return _users;
        }
    }
}
