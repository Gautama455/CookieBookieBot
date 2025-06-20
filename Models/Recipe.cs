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
        public DateTime DateOfCreated { get; private set; }

        [JsonConstructor]
        public Recipe(string name, string description, List<Ingredient> ingredients,
            string category, string difficulty, string author, List<string> steps,
            string image)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Description = description;
            Ingredients = ingredients;
            Category = category;
            Difficulty = difficulty;
            Author = author;
            Steps = steps;
            Image = image;
            DateOfCreated = DateTime.Today;
        }

        [JsonConstructor]
        public Recipe()
        {
            Id = Guid.NewGuid().ToString();
        }

        public void SetName(string name) { Name = name; }
        public void SetDescription(string description) { Description = description; }
        public void SetIngredients(List<Ingredient> ingredients) { Ingredients = ingredients; }
        public void SetCategory(string category) { Category = category; }
        public void SetDifficulty(string difficulty) { Difficulty = difficulty; }
        public void SetAuthor(string authorName) { Author = authorName; }
        public void SetSteps(List<string> steps) { Steps = steps; }
        public void SetImage() { Image = ""; } // TODO добавить функционал для добавления картинок
        public void SetDateOfCreated() { DateOfCreated = DateTime.Today; }

    }
}
