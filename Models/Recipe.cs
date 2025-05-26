namespace CookieBookieBot.Models
{
    internal class Recipe
    {
        public string Name { get; private set; } 
        public string Dascription { get; private set; }
        public List<string> Ingredients { get; private set; }
    }
}
