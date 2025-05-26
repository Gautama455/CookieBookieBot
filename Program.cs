using CookieBookieBot.Commands;

internal class Program
{
    static async Task Main(string[] args)
    {
        string token = "7618715792:AAFS_-1mlpDiDpLyfmmQQ6K0lbhEl-7XJNo";

        var botApp = new BotApplication(token);

        // Запускаем бота
        botApp.Start();

        Console.WriteLine("Бот запущен. Нажмите Enter для выхода.");

        Console.ReadLine();

        // При необходимости можно добавить остановку бота и очистку ресурсов
        // await botApp.StopAsync();
    }
}