using CookieBookieBot.Commands;

class Program
{
    static async Task Main(string[] args)
    {
        string token = "ВАШ_ТОКЕН_БОТА";

        var botApp = new BotApplication(token);

        // Запускаем бота
        botApp.Start();

        Console.WriteLine("Бот запущен. Нажмите Enter для выхода.");

        Console.ReadLine();

        // При необходимости можно добавить остановку бота и очистку ресурсов
        // await botApp.StopAsync();
    }
}