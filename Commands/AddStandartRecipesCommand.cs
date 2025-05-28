using CookieBookieBot.Data;
using Telegram.Bot;

namespace CookieBookieBot.Commands
{
    internal class AddStandartRecipesCommand : BotCommand
    {
        public override string Command => "/add_standrt_recipes";

        public override async Task Run(ITelegramBotClient botClient, Telegram.Bot.Types.Message message, CancellationToken ct)
        {
            try
            {
                JsonBuilder.BuildRecipe(message);
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Стандартные рецепты успешно добавлены",
                    cancellationToken: ct
                    );
            }
            catch (Exception ex)
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Ошибка при добавлени рецепта: {ex.Message}",
                    cancellationToken: ct
                    );
            }
        }
    }
}
