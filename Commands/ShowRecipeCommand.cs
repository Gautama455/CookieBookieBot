using CookieBookieBot.Models;
using Newtonsoft.Json;
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

            List<Recipe> recipes;
            try
            {
                string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent?.Parent?.Parent?.FullName;
                string json = File.ReadAllText($"{Path.Combine(projectRoot, "Data")}\\chats\\{message.Chat.Id}\\standart_recipes_{message.Chat.Id}");
                recipes = JsonConvert.DeserializeObject<List<Recipe>>(json);
            }
            catch (Exception ex)
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Ошибка при чтении базы рецептов: {ex.Message}",
                    cancellationToken: ct
                );
                return;
            }

            var recipe = recipes
           .FirstOrDefault(r => r.Name.Trim().ToLower() == dishName);

            if (recipe != null)
            {
                string response = $"*{recipe.Name}*\n\n" +
                                  $"{recipe.Description}\n\n" +
                                  $"Ингредиенты:\n" +
                                  string.Join("\n", recipe.Ingredients.Select(item =>
                                      $"- {item.Name}: {item.Quantity} {item.Unit}")) +
                                  $"\n\nШаги:\n" +
                                  string.Join("\n", recipe.Steps.Select((step, idx) =>
                                      $"{idx + 1}. {step}"));

                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: response,
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
        public override Task HandleCallbackQuery(ITelegramBotClient botClient, Telegram.Bot.Types.CallbackQuery callbackQuery, CancellationToken ct) => Task.CompletedTask;
    }
}
