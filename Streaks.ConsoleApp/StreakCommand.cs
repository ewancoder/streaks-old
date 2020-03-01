namespace Streaks.ConsoleApp
{
    using System;

    public sealed class StreakCommand
    {
        public StreakCommand(byte streakIndex, StreakCommandAction action)
        {
            if (streakIndex == 0)
                throw new ArgumentException("Streak index should be 1-based.");

            StreakIndex = streakIndex;
            Action = action;
        }

        public byte StreakIndex { get; }
        public StreakCommandAction Action { get; }

        public static StreakCommand TryParse(string input)
        {
            var parts = input.Split(' ');
            if (parts.Length != 2)
                return null;

            Byte.TryParse(parts[0], out byte streakIndex);

            if (streakIndex == 0)
                return null;

            StreakCommandAction action;

            switch (parts[1])
            {
                case "q":
                case "question":
                    action = StreakCommandAction.EditQuestion;
                    break;
                case "y":
                case "yes":
                    action = StreakCommandAction.AnswerYes;
                    break;
                case "n":
                case "no":
                    action = StreakCommandAction.AnswerNo;
                    break;
                default:
                    return null;
            }

            return new StreakCommand(streakIndex, action);
        }
    }
}
