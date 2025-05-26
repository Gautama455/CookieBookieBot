using CookieBookieBot.Commands.Interface;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookieBookieBot.Commands
{

    internal class CommandUpdateHandler<TCommand> : ICommandUpdateHandler where TCommand : BotCommand, new()
    {
        private readonly TCommand _command = new();

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update.Message is not { } message || message.Text is not { } messageText)
                return;

            if (messageText.StartsWith(_command.Command, StringComparison.OrdinalIgnoreCase))
                await _command.Run(botClient, message, ct);
        }
    }
}
