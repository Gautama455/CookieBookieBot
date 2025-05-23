using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookieBookieBot.Commands
{
    internal interface IRunable
    {
        public abstract Task Run(ITelegramBotClient botClient, Message message, CancellationToken ct);
    }
}
