using CookieBookieBot.Models;
using Newtonsoft.Json;


namespace CookieBookieBot.Data
{
    internal class JsonBuilder
    {
        public static void BuildStandartRecipes(Telegram.Bot.Types.Message message)
        {
            string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent?.Parent?.Parent?.FullName;
            Directory.CreateDirectory($"{Path.Combine(projectRoot, "Data")}\\chats\\{message.Chat.Id}");

            Recipe recipe = new Recipe(
            name: "Борщ",
            description: "Классический украинский борщ со свеклой и говядиной.",
            category: "Супы",
            difficulty: "Средний",
            author: message.From?.Username ?? "unknown",
            ingredients: new List<Ingredient>
            {
                new Ingredient("Свекла", 2, "шт"),
                new Ingredient("Картофель", 3, "шт"),
                new Ingredient("Говядина", 400, "г")
            },
            steps: new List<string>
            {
                "Отварить мясо до готовности.",
                "Добавить нарезанные овощи.",
                "Варить до мягкости овощей.",
                "Посолить и поперчить по вкусу."
            },
            image: ""
            );

            List<Recipe> recipes = new List<Recipe>();

            recipes.Add(recipe);

            string json = JsonConvert.SerializeObject(recipes, Formatting.Indented);

            File.WriteAllText($"{Path.Combine(projectRoot, "Data")}\\chats\\{message.Chat.Id}\\standart_recipes_{message.Chat.Id}", json);
        }
    }
}
