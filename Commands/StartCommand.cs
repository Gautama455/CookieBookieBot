using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CookieBookieBot.Commands
{
    internal class StartCommand : BotCommand
    {
        public override string Command => "/start";

        public override async Task Run(ITelegramBotClient botClient, Telegram.Bot.Types.Message message, CancellationToken ct)
        {
            string messageText = "Привет! Я бот по управлению рецептами \n\n" +
                "Уменю выполнять следующие команды:\n" +
                "\t/add - Добавитть рецепт\n" +
                "\t/list - показать все рецепты";

            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: messageText,
                parseMode: ParseMode.Markdown,
                cancellationToken: ct
                );
        }
        public override Task HandleCallbackQuery(ITelegramBotClient botClient, Telegram.Bot.Types.CallbackQuery callbackQuery, CancellationToken ct) => Task.CompletedTask;
    }
}
