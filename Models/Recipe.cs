using System.Text.Json.Serialization;

namespace CookieBookieBot.Models
{
    internal class Recipe
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<Ingredient> Ingredients { get; private set; }
        public string Category { get; private set; }
        public string Difficulty { get; private set; }
        public string Author { get; private set; }
        public List<string> Steps { get; private set; }
        public string Image { get; private set; }
        public string DateOfCreated { get; private set; }

        [JsonConstructor]
        public Recipe(string id, string name, string description, List<Ingredient> ingredients,
            string category, string difficulty, string author, List<string> steps,
            string image, string dateOfCreated)
        {
            Id = id;
            Name = name;
            Description = description;
            Ingredients = ingredients;
            Category = category;
            Difficulty = difficulty;
            Author = author;
            Steps = steps;
            Image = image;
            DateOfCreated = dateOfCreated;
        }
    }
}
