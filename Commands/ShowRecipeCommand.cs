using Telegram.Bot;

namespace CookieBookieBot.Commands
{
    internal class ShowRecipeCommand : BotCommand
    {
        public override string Command => "/recipe";

        public override async Task Run(ITelegramBotClient botClient, Telegram.Bot.Types.Message message, CancellationToken ct)
        {
            var parts = message.Text.Split(
                separator: ' ',
                count: 2,
                options: StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Пожалуйста, используйте команду в формате:\n/showRecipe [название блюда]",
                    cancellationToken: ct
                    );
                return;
            }

            string dishName = parts[1].Trim().ToLower();

            if (RecipeStorage.Recipe.TryGetValue(dishName, out var recipe))
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Рецепт для \"{dishName}\":\n{recipe}",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: ct
                    );
            }
            else
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Рецепт для \"{dishName}\" не найден.",
                    cancellationToken: ct
                    );
            }
        }
    }
}
