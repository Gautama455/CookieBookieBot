using CookieBookieBot.Models;
using Newtonsoft.Json;
using Telegram.Bot;

namespace CookieBookieBot.Commands
{
    internal class AddNewRecipeCommand : BotCommand
    {
        public override string Command => "add_recipe";

        public override async Task Run(ITelegramBotClient botClient, Telegram.Bot.Types.Message message, CancellationToken ct)
        {
            // Ожидаем команду вида: /addrecipe Название; Описание; Категория; Сложность; Ингредиенты; Шаги
            var parts = message.Text.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Используйте формат:\n/addrecipe Название; Описание; Категория; Сложность; Ингредиенты; Шаги",
                    cancellationToken: ct);
                return;
            }

            var fields = parts[1].Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length < 6)
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Не хватает параметров. Формат:\n/addrecipe Название; Описание; Категория; Сложность; Ингредиенты; Шаги",
                    cancellationToken: ct);
                return;
            }

            string name = fields[0].Trim();
            string description = fields[1].Trim();
            string category = fields[2].Trim();
            string difficulty = fields[3].Trim();
            string ingredientsRaw = fields[4].Trim();
            string stepsRaw = fields[5].Trim();

            // Парсим ингредиенты
            var ingredients = new List<Ingredient>();
            foreach (var ing in ingredientsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var ingParts = ing.Split(':');
                if (ingParts.Length != 3)
                    continue;
                ingredients.Add(new Ingredient(
                    quantity: int.TryParse(ingParts[1], out int q) ? q : 0,
                    unit: ingParts[2],
                    name: ingParts[0]
                ));
            }

            // Парсим шаги
            var steps = stepsRaw.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();

            // Генерируем уникальный Id
            string id = Guid.NewGuid().ToString();

            // Читаем существующие рецепты
            string dataPath = Path.Combine(AppContext.BaseDirectory, "Data", "recipes.json");
            List<Recipe> recipes = new List<Recipe>();
            if (File.Exists(dataPath))
            {
                string json = File.ReadAllText(dataPath);
                if (!string.IsNullOrWhiteSpace(json))
                    recipes = JsonConvert.DeserializeObject<List<Recipe>>(json) ?? new List<Recipe>();
            }

            // Проверка на дублирование по имени
            if (recipes.Any(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Рецепт с названием \"{name}\" уже существует.",
                    cancellationToken: ct);
                return;
            }

            // Добавляем новый рецепт
            var recipe = new Recipe(
                id: id,
                name: name,
                description: description,
                category: category,
                difficulty: difficulty,
                author: message.From?.Username ?? "unknown",
                ingredients: ingredients,
                steps: steps,
                image: "",
                dateOfCreated: DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            );
            recipes.Add(recipe);

            // Сохраняем обратно в файл
            Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
            File.WriteAllText(dataPath, JsonConvert.SerializeObject(recipes, Formatting.Indented));

            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Рецепт \"{name}\" успешно добавлен!",
                cancellationToken: ct);
        }
    }
}
