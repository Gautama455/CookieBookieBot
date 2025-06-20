using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookieBookieBot.Commands.Interface
{
    internal interface ISessionCommand
    {
        Task HandleUserResponse(ITelegramBotClient botClient, Message message, CancellationToken ct);
        bool IsSessionComplete { get; }
    }
}
