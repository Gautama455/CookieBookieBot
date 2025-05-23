using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookieBookieBot.Commands
{
    internal abstract class BotCommand : IRunable
    {
        public abstract string Command { get; }
        public abstract Task Run(ITelegramBotClient botClient, Message message, CancellationToken ct);
    }
}
