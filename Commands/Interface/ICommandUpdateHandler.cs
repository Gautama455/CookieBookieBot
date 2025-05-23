using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookieBookieBot.Commands.Interface
{
    internal interface ICommandUpdateHandler
    {
        Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken ct);
    }
}
