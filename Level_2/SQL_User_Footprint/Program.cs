using System;
using System.Threading;

namespace MySqlUserFootprint {
  class Program {
    static void Main(string[] args) {
      Config config = Config.LoadFromFile("config.json");

      //Инициализация логгирования
      Logger.Initialize("log.txt");

      //Подключение к БД
      try {
        Database db = new Database(config.Host, config.User, config.Password, condig.Database);
        db.Connsect();

        //Логика отслеживания
        while (true) {
          Console.WriteLine("Имитация активности пользователя...");

          string userAction = Console.ReadLine();
          UserActivity activity = new UserActivity {
            //Может быть небезопасно
            User = Enviroment.UserName, Action = userAction, Details = "Дополнительные детали"
          };

          try {
            db.LogUserActivity(activity);
            Console.WriteLine("Действие пользователя записано.");
          }
          catch (Exception ex) {
            Logger.LogError("Ошибка записи в БД: " + ex.Message);
            Console.WriteLine("Ошибка записи в БД. Подробности в лог-файле.");
          }
          //Пауза 5 секунд
          Thread.Sleep(5000);
        }
      }

      catch (Exception ex) {
        Logger.LogError("Критическая ошибка: " + ex.Message);
        Console.WriteLine("Критическая ошибка. Подробности в лог-файле.");
      }
      finally {
        Logger.Close();
        Console.WriteLine("Завершение работы программы. Нажмите любую клавишу...");

        Console.ReadKey();
      }
    }
  }
}
