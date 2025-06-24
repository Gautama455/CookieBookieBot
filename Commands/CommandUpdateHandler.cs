using CookieBookieBot.Commands.Interface;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookieBookieBot.Commands
{

    internal class CommandUpdateHandler<TCommand> : ICommandUpdateHandler where TCommand : BotCommand, new()
    {
        private static readonly Dictionary<long, TCommand> _sessions = new();

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update.Message is { } message && !string.IsNullOrEmpty(message.Text))
            {
                var chatId = message.Chat.Id;

                if (message.Text.StartsWith(new TCommand().Command, StringComparison.OrdinalIgnoreCase))
                {
                    var commandInstance = new TCommand();
                    _sessions[chatId] = commandInstance;
                    await commandInstance.Run(botClient, message, ct);
                    return;
                }

                if (_sessions.TryGetValue(chatId, out var sessionCommand))
                {
                    if (sessionCommand is ISessionCommand session)
                    {
                        await session.HandleUserResponse(botClient, message, ct);

                        if (session.IsSessionComplete)
                        {
                            _sessions.Remove(chatId);
                        }
                    }
                }
            } 
            else if (update.CallbackQuery is { } callBack)
            {
                if (_sessions.TryGetValue(callBack.Message.Chat.Id, out var sessionCommand))
                {
                    await sessionCommand.HandleCallbackQuery(botClient, callBack, ct);

                    if (sessionCommand is ISessionCommand session && session.IsSessionComplete)
                    {
                        _sessions.Remove(callBack.Message.Chat.Id);
                    }
                }
            }
        }
    }
}
