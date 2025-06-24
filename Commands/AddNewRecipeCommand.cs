using CookieBookieBot.Commands.Interface;
using CookieBookieBot.Models;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookieBookieBot.Commands
{
    internal class AddNewRecipeCommand : BotCommand, ISessionCommand
    {
        public override string Command => "/add_new";

        private enum InputStep
        {
            Name = 1,
            Description,
            Category,
            Difficulty,
            Ingredients,
            Steps,
            Confirmation,
            Completed
        }

        private InputStep _currentStep = InputStep.Name;

        private Recipe _recipe = new Recipe();
        private List<Ingredient> _ingredients = new();
        private List<string> _steps = new();

        public bool IsSessionComplete => _currentStep == InputStep.Completed;
        public override async Task Run(ITelegramBotClient botClient, Telegram.Bot.Types.Message message, CancellationToken ct)
        {
            _currentStep = InputStep.Name;
            _recipe = new Recipe();
            _ingredients = new List<Ingredient>();
            _steps = new List<string>();

            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: "Введите название рецепта:",
                replyMarkup: new ForceReplyMarkup { Selective = true },
                cancellationToken: ct);
        }

        public async Task HandleUserResponse(ITelegramBotClient botClient, Telegram.Bot.Types.Message message, CancellationToken ct)
        {
            var chatId = message.Chat.Id;
            var text = message.Text?.Trim();

            if (string.IsNullOrEmpty(text))
            {
                await botClient.SendMessage(chatId, "Пустой ввод, попробуйте ещё раз.", cancellationToken: ct);
                return;
            }

            switch (_currentStep)
            {
                case InputStep.Name:
                    _recipe.SetName(text);
                    _currentStep = InputStep.Description;
                    await botClient.SendMessage(chatId, "Введите описание рецепта:", replyMarkup: new ForceReplyMarkup { Selective = true }, cancellationToken: ct);
                    break;

                case InputStep.Description:
                    _recipe.SetDescription(text);
                    _currentStep = InputStep.Category;
                    var categories = new[] { "Супы", "Основные блюда", "Десерты", "Закуски" };
                    await botClient.SendMessage(chatId, "Выберите категорию:", replyMarkup: new ReplyKeyboardMarkup(categories.Select(c => new KeyboardButton(c))) { ResizeKeyboard = true, OneTimeKeyboard = true }, cancellationToken: ct);
                    break;

                case InputStep.Category:
                    var validCategories = new HashSet<string> { "Супы", "Основные блюда", "Десерты", "Закуски" };
                    if (!validCategories.Contains(text))
                    {
                        await botClient.SendMessage(chatId, "Пожалуйста, выберите категорию из предложенных вариантов.", cancellationToken: ct);
                        return;
                    }
                    _recipe.SetCategory(text);
                    _currentStep = InputStep.Difficulty;
                    var difficulties = new[] { "Лёгко", "Средне", "Сложно" };
                    await botClient.SendMessage(chatId, "Выберите сложность:", replyMarkup: new ReplyKeyboardMarkup(difficulties.Select(d => new KeyboardButton(d))) { ResizeKeyboard = true, OneTimeKeyboard = true }, cancellationToken: ct);
                    break;

                case InputStep.Difficulty:
                    var validDifficulties = new HashSet<string> { "Лёгко", "Средне", "Сложно" };
                    if (!validDifficulties.Contains(text))
                    {
                        await botClient.SendMessage(chatId, "Пожалуйста, выберите сложность из предложенных вариантов.", cancellationToken: ct);
                        return;
                    }
                    _recipe.SetDifficulty(text);
                    _currentStep = InputStep.Ingredients;
                    await botClient.SendMessage(chatId,
                        "Введите ингредиенты по одному в формате: Название:Количество:Единица\nНапример: Мука:200:г\nОтправьте 'Готово', когда закончите.",
                        replyMarkup: new ReplyKeyboardMarkup(new[] { new KeyboardButton("Готово") }) { ResizeKeyboard = true, OneTimeKeyboard = true },
                        cancellationToken: ct);
                    break;

                case InputStep.Ingredients:
                    if (text.Equals("Готово", StringComparison.OrdinalIgnoreCase))
                    {
                        if (_ingredients.Count == 0)
                        {
                            await botClient.SendMessage(chatId, "Пожалуйста, введите хотя бы один ингредиент.", cancellationToken: ct);
                            return;
                        }
                        _recipe.SetIngredients(_ingredients);
                        _currentStep = InputStep.Steps;
                        await botClient.SendMessage(chatId,
                            "Введите шаги приготовления по одному.\nОтправьте 'Готово', когда закончите.",
                            replyMarkup: new ReplyKeyboardMarkup(new[] { new KeyboardButton("Готово") }) { ResizeKeyboard = true, OneTimeKeyboard = true },
                            cancellationToken: ct);
                        break;
                    }
                    // Парсим ингредиент
                    var parts = text.Split(':');
                    if (parts.Length != 3 || !int.TryParse(parts[1], out int quantity))
                    {
                        await botClient.SendMessage(chatId,
                            "Неверный формат. Используйте: Название:Количество:Единица\nНапример: Мука:200:г",
                            cancellationToken: ct);
                        return;
                    }
                    _ingredients.Add(new Ingredient(parts[0].Trim(), quantity, parts[2].Trim()));
                    await botClient.SendMessage(chatId, "Ингредиент добавлен. Введите следующий или 'Готово' для завершения.", cancellationToken: ct);
                    break;

                case InputStep.Steps:
                    if (text.Equals("Готово", StringComparison.OrdinalIgnoreCase))
                    {
                        if (_steps.Count == 0)
                        {
                            await botClient.SendMessage(chatId, "Пожалуйста, введите хотя бы один шаг.", cancellationToken: ct);
                            return;
                        }
                        _recipe.SetSteps(_steps);
                        _currentStep = InputStep.Confirmation;

                        string summary = $"Проверьте рецепт:\n" +
                                         $"Название: {_recipe.Name}\n" +
                                         $"Описание: {_recipe.Description}\n" +
                                         $"Категория: {_recipe.Category}\n" +
                                         $"Сложность: {_recipe.Difficulty}\n" +
                                         $"Ингредиенты: {_ingredients.Count}\n" +
                                         $"Шаги: {_steps.Count}\n\n" +
                                         $"Сохранить рецепт?";

                        var confirmKeyboard = new InlineKeyboardMarkup(new[]
                        {
                        InlineKeyboardButton.WithCallbackData("Сохранить", "save_recipe"),
                        InlineKeyboardButton.WithCallbackData("Отмена", "cancel_recipe")
                    });

                        await botClient.SendMessage(chatId, summary, replyMarkup: confirmKeyboard, cancellationToken: ct);
                    }
                    else
                    {
                        _steps.Add(text);
                        await botClient.SendMessage(chatId, "Шаг добавлен. Введите следующий или 'Готово' для завершения.", cancellationToken: ct);
                    }
                    break;

                case InputStep.Confirmation:
                    // Ожидается обработка callback, здесь можно игнорировать или напомнить пользователю
                    await botClient.SendMessage(chatId, "Пожалуйста, используйте кнопки для выбора действия.", cancellationToken: ct);
                    break;

                default:
                    await botClient.SendMessage(chatId, "Неизвестный шаг. Попробуйте начать заново.", cancellationToken: ct);
                    break;
            }
        }

        public override async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken ct)
        {
            var chatId = callbackQuery.Message.Chat.Id;

            if (_currentStep != InputStep.Confirmation)
            {
                await botClient.AnswerCallbackQuery(callbackQuery.Id, "Нет активного подтверждения.", cancellationToken: ct);
                return;
            }

            if (callbackQuery.Data == "save_recipe")
            {
                // Генерация id и заполнение остальных полей
                _recipe.SetAuthor(callbackQuery.From.Username ?? "unknown");
                _recipe.SetImage();
                _recipe.SetDateOfCreated();

                // Сохранение рецепта (пример с файлом)
                string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent?.Parent?.Parent?.FullName;
                string dataPath = Path.Combine(projectRoot, "Data", "chats" , chatId.ToString(), "recipes.json");
                Directory.CreateDirectory(dataPath);
                List<Recipe> recipes = new();
                if (File.Exists(dataPath))
                {
                    var json = await File.ReadAllTextAsync(dataPath, ct);
                    if (!string.IsNullOrWhiteSpace(json))
                        recipes = JsonConvert.DeserializeObject<List<Recipe>>(json) ?? new();
                }

                if (recipes.Any(r => r.Name.Equals(_recipe.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    await botClient.SendMessage(chatId, $"Рецепт с названием \"{_recipe.Name}\" уже существует.", cancellationToken: ct);
                    return;
                }

                recipes.Add(_recipe);
                Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
                await File.WriteAllTextAsync(dataPath, JsonConvert.SerializeObject(recipes, Formatting.Indented), ct);

                await botClient.SendMessage(chatId, $"Рецепт \"{_recipe.Name}\" успешно добавлен!", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: ct);

                _currentStep = InputStep.Completed;
            }
            else if (callbackQuery.Data == "cancel_recipe")
            {
                await botClient.SendMessage(chatId, "Добавление рецепта отменено.", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: ct);
                _currentStep = InputStep.Completed;
            }

            await botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
        }
    }
}
