using CookieBookieBot.Commands;
using CookieBookieBot.Commands.Interface;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookieBookieBot
{
    internal class BotApplication
    {
        private readonly ITelegramBotClient _botClient;
        private readonly List<ICommandUpdateHandler> _handlers = new();

        public BotApplication(string token)
        {
            _botClient = new TelegramBotClient(token);

            _handlers.Add(new CommandUpdateHandler<StartCommand>());
            _handlers.Add(new CommandUpdateHandler<ShowRecipeCommand>());
            _handlers.Add(new CommandUpdateHandler<AddStandartRecipesCommand>());
            _handlers.Add(new CommandUpdateHandler<AddNewRecipeCommand>());
        }

        public void Start()
        {
            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandlePollingErrorAsync);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            foreach (var handler in _handlers)
            {
                await handler.HandleUpdate(botClient, update, ct);
            }
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
