using Microsoft.AspNetCore.Mvc;
using SQLiTrainingApp.Models;

namespace SQLiTrainingApp.Controllers
{
    public class TrainingController : Controller
    {
        private readonly Database _database;

        public TrainingController(Database database)
        {
            _database = database;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password, bool useSafeMethod = false)
        {
            User user;
            
            if (useSafeMethod)
            {
                user = _database.SafeGetUser(username, password);
                ViewBag.Message = "Безопасный метод: параметризованный запрос";
            }
            else
            {
                user = _database.VulnerableGetUser(username, password);
                ViewBag.Message = "Уязвимый метод: прямое включение в запрос";
            }

            if (user != null)
            {
                ViewBag.Success = $"Успешный вход! Добро пожаловать, {user.Username} (Роль: {user.Role})";
            }
            else
            {
                ViewBag.Error = "Неверные учетные данные!";
            }

            return View("Index");
        }

        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            var results = _database.VulnerableSearchUsers(searchTerm);
            ViewBag.SearchResults = results;
            ViewBag.SearchTerm = searchTerm;
            
            return View("Index");
        }

        [HttpPost]
        public IActionResult ResetDatabase()
        {
            // В реальном приложении здесь была бы переинициализация БД
            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "Data", "database.json"));
            _database.InitializeDatabase();
            
            ViewBag.Message = "База данных重置成功!";
            return View("Index");
        }
    }
}
