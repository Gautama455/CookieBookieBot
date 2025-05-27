using CookieBookieBot.Models;
using Newtonsoft.Json;


namespace CookieBookieBot.Data
{
    internal class JsonBuilder
    {
        public static void BuildRecipe()
        {
            var recipe = new Recipe(
            id: "borsch-001",
            name: "Борщ",
            description: "Классический украинский борщ со свеклой и говядиной.",
            category: "Супы",
            difficulty: "Средний",
            author: "Ivan Ivanov",
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
            image: "https://example.com/images/borscht.jpg",
            dateOfCreated: "ffff"
        );
            string json = JsonConvert.SerializeObject(recipe, Formatting.Indented);

            File.WriteAllText("recipe.json", json);
        }
    }
}
