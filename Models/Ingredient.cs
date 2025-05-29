using System.Text.Json.Serialization;

namespace CookieBookieBot.Models
{
    internal class Ingredient
    {
        public string Name { get; private set; }
        public int Quantity { get; private set; }
        public string Unit { get; private set; }

        [JsonConstructor]
        public Ingredient(string name, int quantity, string unit)
        {
            Name = name;
            Quantity = quantity;
            Unit = unit;
        }
    }
}
