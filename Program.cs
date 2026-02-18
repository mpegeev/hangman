const int MAX_STEPS = 10;
char[] allLetters = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя".ToCharArray(); // все буквы алфавита

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Тебя приветствует игра");
Console.WriteLine($"""

                ╭┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄╮
                │                                           │
                │                                           │
                │              В И С Е Л И Ц А              │
                │                                           │
                │                                           │
                ╰┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄╯

""");

Console.WriteLine("Введи слово или фразу от 3 до 50 букв, которое будем угадывать.");

string phrase = null!;
bool inputValid = false;
while (!inputValid) // валидируем загаданное слово
{
    var wordInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(wordInput)) // одни пробелы или ничего
    {
        inputValid = false;
    }
    else
    {
        var normalized = wordInput.Trim().ToLowerInvariant(); // убираем пробелы на концах и переводим в нижний регистр
        if (!normalized.All(IsValidLetter)) // проверяем, все ли буквы из алфавита
        {
            inputValid = false; // увы, не все :(
        }
        else
        {
            inputValid = 3 <= normalized.Length && normalized.Length <= 50; // проверяем длину слова
            phrase = normalized; // записываем в переменную, с которой будем дальше работать
        }
    }

    if (!inputValid) Console.WriteLine("Упс, похоже не получилось. Попробуй ещё раз.");
}

// общаемся с пользователем
Console.Clear();
Console.WriteLine("А теперь зови друга и жми на любую кнопку.");
Console.ReadKey();
Console.Clear();
Console.WriteLine("Друг, скажи, как тебя зовут?");
var playerName = Console.ReadLine();
Console.Clear();
if (string.IsNullOrWhiteSpace(playerName))
    playerName = "Неизвестный герой";
var wordsInPhrase = phrase.Split(' ');
Console.WriteLine($"Отлично! Приятно познакомиться! {playerName}, тебе нужно угадать фразу из {wordsInPhrase.Length} слов и {wordsInPhrase.Sum(word => word.Length)} букв.");
Console.WriteLine($"Если справишься, то молодец, а если нет, то тебя вздёрнут на ВИСЕЛИЦЕ.");
Console.WriteLine($"Вперёд!\n\nНажми любую кнопку.");
Console.ReadKey();
Console.Clear();

int currentTry = 1;
char[] userLetters = new char[char.MaxValue + 1]; // буквы, введённые пользователем
bool[] matchedLetters = new bool[char.MaxValue + 1]; // буквы, угаданные пользователем. Если буквы нет в загаданном слове, то она так и останется "не угаданной"
foreach (var letter in allLetters) // готовим массив всех букв
{
    userLetters[letter] = '-'; // для вывода в консоль, пока не ввели букву, здесь будет прочерк. Когда пользователь введёт букву, то здесь будет введённая буква
    matchedLetters[letter] = false; // пока ничего не угадали
}

while (currentTry <= 10) // начинаем игру
{
    DrawGallows(currentTry); // выводим виселицу
    DrawMatchedLetters(); // выводим названные буквы
    DrawMatchedWord(); // выводим слово с угаданными буквами

    Console.WriteLine($"\n\nВведи букву или фразу целиком");
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput)) // пусто (нажали Enter либо одни пробелы)
    {
        DrawAnswerAtTop("Как-то пустовато.. Давай ещё разок.");
        continue;
    }
    if (userInput.Length == 1) // ввели букву, проверяем
    {
        var letter = char.Parse(userInput.ToLowerInvariant());
        if (!IsValidLetter(letter))
        {
            DrawAnswerAtTop("Я не нашёл такую букву у себя. Используй кириллицу.");
            continue;
        }
        if (userLetters.Contains(letter))
        {
            DrawAnswerAtTop("Ты уже называл такую, освежи свою память и попробуй ещё раз.");
            continue;
        }
        userLetters[letter] = letter; // ввели букву, кладём в массив
        if (phrase.Contains(letter)) // такая буква есть в слове
        {
            matchedLetters[letter] = true;
            if (IsSuccess()) // проверяем, не отгадано ли слово
            {
                DrawSuccessAndWait();
                return;
            }

            // слово не отгадано, а вот буква да. Попытку не добавляем
            DrawAnswerAtTop("Есть такая буква!");
            continue;
        }

        // не угадал, добавляем попытку
        DrawAnswerAtTop("Нет такой буквы!");
        currentTry++;
        continue;
    }
    if (userInput.Length != phrase.Length) // неправильная длина слова
    {
        DrawAnswerAtTop("Ошибся с длиной слова, нещитово! Пробуй снова.");
        continue;
    }
    if (userInput.Length == phrase.Length) // ввели слово, проверяем
    {
        if (string.Compare(userInput, phrase, StringComparison.OrdinalIgnoreCase) == 0) // угадал
        {
            DrawSuccessAndWait();
            return;
        }

        // не угадал, добавляем попытку
        DrawAnswerAtTop("Не угадал!");
        currentTry++;
        continue;
    }
}

// попытки закончились
DrawAnswerAtTop($"Загаданная фраза: {phrase}");
DrawGallows(11);
Console.ReadKey();

bool IsValidLetter(char letter) => allLetters.Contains(letter) || letter == ' '; // пробел либо буква из алфавита
bool IsSuccess() // проверяем, угадано ли слово
{
    bool isSuccess = true;
    foreach (var item in phrase.Split(' '))
    {
        isSuccess = isSuccess && item.All(userLetters.Contains);
    }
    return isSuccess;
}

void DrawAnswerAtTop(string message) // выводим сообщение от игры
{
    Console.Clear();
    Console.WriteLine($"\t{message}");
}
void DrawMatchedLetters() // выводим названные букв
{
    Console.Write("\nНазванные буквы: ");
    foreach (var letter in allLetters) Console.Write(userLetters.Contains(letter) ? letter : "-");
}
void DrawMatchedWord() // выводим слово с угаданными буквами
{
    Console.Write("\nСлово: ");
    foreach (var letter in phrase)
        Console.Write((matchedLetters[letter] || letter == ' ') ? letter : "-");
}

// шаблон того, как выглядит виселица
string _ = $"""
    
                    ╭┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄╮top line
                    │          ОСТАЛОСЬ N  ходов             │remaining steps
                    │                                        │empty line
                    │             ┏━━━┓                      │line 1
                    │             ┃   ☻                      │line 2
                    │             ┃  ╱│╲                     │line 3
                    │             ┃  ╱ ╲                     │line 4
                    │            ━┻━                         │line 5
                    ╰┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄╯bottom line

""";

const string TopLine = "                ╭┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄╮";
const string EmptyLine = "                │                                        │";
const string BottomLine = "                ╰┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄╯";

void DrawGallows(int @try) // выводим изображение виселицы в зависимости от текущей попытки
{
    var remainingStepsLine = GetRemainingStepsLine(MAX_STEPS - @try + 1);
    Console.WriteLine($@"
{TopLine}
{remainingStepsLine}
{EmptyLine}
{GetLine1(@try)}
{GetLine2(@try)}
{GetLine3(@try)}
{GetLine4(@try)}
{GetLine5(@try)}
{BottomLine}
    ");
}

string GetRemainingStepsLine(int remainigSteps) // получаем строку с количеством оставшихся попыток
{
    var charNumber = (char)(0x2775 + remainigSteps);
    return remainigSteps switch
    {
        0 => $"                │          ТЕБЯ ПОВЕСИЛИ!                │",
        1 => $"                │          ОСТАЛСЯ {charNumber}  ход                │",
        2 => $"                │          ОСТАЛОСЬ {charNumber}  хода              │",
        _ => $"                │          ОСТАЛОСЬ {charNumber}  ходов             │",
    };
}

string GetLine5(int @try) => @try switch
{
    1 => EmptyLine,
    _ => "                │            ━┻━                         │",
};

string GetLine4(int @try) => @try switch
{
    1 => EmptyLine,
    2 => EmptyLine,
    3 => "                │             ┃                          │",
    4 => "                │             ┃                          │",
    5 => "                │             ┃                          │",
    6 => "                │             ┃                          │",
    7 => "                │             ┃                          │",
    8 => "                │             ┃                          │",
    9 => "                │             ┃                          │",
   10 => "                │             ┃                          │",
    _ => "                │             ┃  ╱ ╲                     │",
};

string GetLine3(int @try) => @try switch
{
    1 => EmptyLine,
    2 => EmptyLine,
    3 => EmptyLine,
    4 => "                │             ┃                          │",
    5 => "                │             ┃                          │",
    6 => "                │             ┃                          │",
    7 => "                │             ┃                          │",
    8 => "                │             ┃                          │",
    9 => "                │             ┃                          │",
   10 => "                │             ┃                          │",
    _ => "                │             ┃  ╱│╲                     │",
};

string GetLine2(int @try) => @try switch
{
    1 => EmptyLine,
    2 => EmptyLine,
    3 => EmptyLine,
    4 => EmptyLine,
    5 => "                │             ┃                          │",
    6 => "                │             ┃                          │",
    7 => "                │             ┃                          │",
    8 => "                │             ┃                          │",
    9 => "                │             ┃                          │",
   10 => "                │             ┃   ☻                      │",
    _ => "                │             ┃   ☻                      │",
};

string GetLine1(int @try) => @try switch
{
    1 => EmptyLine,
    2 => EmptyLine,
    3 => EmptyLine,
    4 => EmptyLine,
    5 => EmptyLine,
    6 => "                │             ┏                          │",
    7 => "                │             ┏━━                        │",
    8 => "                │             ┏━━━                       │",
    9 => "                │             ┏━━━┓                      │",
   10 => "                │             ┏━━━┓                      │",
    _ => "                │             ┏━━━┓                      │",
};

void DrawSuccessAndWait() // выводим победное окно
{
    Console.Clear();
    Console.WriteLine($"""

                ╭┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄╮
                │                                           │
                │                                           │
                │                П О Б Е Д А !              │
                │                                           │
                │                                           │
                ╰┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄╯

                     Загаданное слово: {phrase}

             На этот раз тебе повезло.. Но ничего, мы ещё поквитаемся!
""");
    Console.ReadKey();
}