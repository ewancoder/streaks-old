namespace Streaks.ConsoleApp
{
    using System;
    using System.IO;
    using System.Linq;

    static class Program
    {
        private static readonly string DataFileName = "data.db";
        private static readonly StreakManagerSerializer _serializer = new StreakManagerSerializer(DataFileName);
        private static StreakManager _streakManager;

        static void Main()
        {
            InitializeStreakManager();

            while (true)
            {
                Console.Clear();
                PrintGreetings();
                PrintState();

                var input = TakeInput();

                ProcessInput(input);

                Save();
            }
        }

        private static void InitializeStreakManager()
        {
            if (!File.Exists(DataFileName))
            {
                var endOfDayOffset = AskEndOfDayOffset();

                _streakManager = new StreakManager
                {
                    EndOfDayOffset = endOfDayOffset
                };

                Save();
                return;
            }

            Load();
        }

        private static void Load()
        {
            _streakManager = _serializer.Load();
        }

        private static void Save()
        {
            _serializer.Save(_streakManager);
        }

        private static void ChangeEndOfDayOffset()
        {
            var endOfDay = AskEndOfDayOffset();

            _streakManager.EndOfDayOffset = endOfDay;
        }

        private static TimeSpan AskEndOfDayOffset()
        {
            while (true)
            {
                try
                {
                    Console.Clear();
                    Console.Write("What is your end of day offset (in hours): if you're going to bed at 4am, the offset would be +4: ");
                    var offset = Console.ReadLine();

                    var result = int.Parse(offset);

                    if (result <= -24 || result >= 24)
                    {
                        Console.Write("The offset should be between -24..+24 hours exclusively.");
                        Console.ReadLine();
                        continue;
                    }

                    return TimeSpan.FromHours(result);
                }
                catch
                {
                    Console.Write("The offset has invalid format.");
                    Console.ReadLine();
                }
            }
        }

        private static void PrintGreetings()
        {
            var left = _streakManager.GetTimeLeftUntilTheEndOfDay();

            Console.WriteLine("===== STREAKS =====");
            Console.WriteLine();
            Console.WriteLine($"Time left: {left.Hours.ToString("0")}:{left.Minutes.ToString("00")}");
            Console.WriteLine();
        }

        private static void PrintState()
        {
            if (_streakManager.GetStreaksCount() == 0)
                Console.WriteLine("You have no streaks.");

            var streaks = _streakManager.GetState().ToList();

            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var streak in streaks.Where(s => s.IsDoneToday))
            {
                Console.WriteLine(Format(streak));
            }

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var streak in streaks.Where(s => !s.IsDoneToday))
            {
                Console.WriteLine(Format(streak));
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        private static string Format(StreakState state)
        {
            return $"{state.Index} - {state.Question} - [ {state.Length} ] - {(state.IsDoneToday ? "yes" : "no")}";
        }

        private static string TakeInput()
        {
            Console.Write("> ");
            return Console.ReadLine();
        }

        private static void ProcessInput(string input)
        {
            switch (input)
            {
                case "add":
                    AddStreak();
                    return;
                case "exit":
                    Environment.Exit(0);
                    return;
                case "help":
                    ShowHelp();
                    return;
                case "end":
                    ChangeEndOfDayOffset();
                    return;
                case "clear-I-know-what-I-am-doing":
                    File.Delete(DataFileName);
                    _streakManager.ClearStreaks();
                    return;
            }

            var command = StreakCommand.TryParse(input);

            if (command == null)
            {
                Console.Write($"There's no such command: {input}. (press [ENTER] to continue)");
                Console.ReadLine();
                return;
            }

            RunCommand(command);
        }

        private static void AddStreak()
        {
            Console.Write("Question: ");
            var question = Console.ReadLine();

            _streakManager.AddStreak(question);
        }

        private static void RunCommand(StreakCommand command)
        {
            if (command.StreakIndex > _streakManager.GetStreaksCount())
            {
                Console.WriteLine($"Streak with index {command.StreakIndex} does not exist.");
                Console.ReadLine();
                return;
            }

            switch (command.Action)
            {
                case StreakCommandAction.EditQuestion:
                    Console.Write("New question: ");
                    _streakManager.UpdateQuestion(command.StreakIndex, Console.ReadLine());
                    break;
                case StreakCommandAction.AnswerYes:
                    _streakManager.Yes(command.StreakIndex);
                    break;
                case StreakCommandAction.AnswerNo:
                    _streakManager.No(command.StreakIndex);
                    break;
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("=== Streaks help ===");
            Console.WriteLine();
            Console.WriteLine("help - Show help.");
            Console.WriteLine("add - Add new streak.");
            Console.WriteLine("exit - Exit the program.");
            Console.WriteLine("clear-I-know-what-I-am-doing - Erase the whole database, start from scratch.");
            Console.WriteLine();
            Console.WriteLine("[number] [command] - Perform a command against specific streak. For example, '1 q' will edit the question of the first streak. Command list is given below:");
            Console.WriteLine("- t, title - change streak title.");
            Console.WriteLine("- q, question - change streak question.");
            Console.WriteLine("- y, yes - mark streak as done today.");
            Console.WriteLine("- n, no - mark streak as not done today.");
            Console.WriteLine();
            Console.Write("(Press [ENTER] to continue)");
            Console.ReadLine();
        }
    }
}
