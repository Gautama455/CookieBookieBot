using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookieBookieBot.Models
{
    internal class AddRecipeSession
    {
        public Recipe Recipe { get; private set; }
        public int SessionStep { get; private set; }

        public AddRecipeSession(Recipe recipe)
        {
            Recipe = recipe;
            SessionStep = 1;
        }
    }
}
