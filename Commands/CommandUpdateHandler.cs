using CookieBookieBot.Commands.Interface;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookieBookieBot.Commands
{

    internal class CommandUpdateHandler<TCommand> : ICommandUpdateHandler where TCommand : BotCommand, ISessionCommand, new()
    {
        private readonly TCommand _command = new();

        private static readonly Dictionary<long, TCommand> _sessions = new();

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update.Message is not { } message || message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            if (messageText.StartsWith(_command.Command, StringComparison.OrdinalIgnoreCase))
            {
                var commandInstance = new TCommand();
                _sessions[chatId] = commandInstance;
                await commandInstance.Run(botClient, message, ct);
                return;
            }

            if (_sessions.TryGetValue(chatId, out var sessionCommand))
            {
                await sessionCommand.HandleUserResponse(botClient, message, ct);

                if (sessionCommand.IsSessionComplete)
                {
                    _sessions.Remove(chatId);
                }
            }
        }
    }
}
