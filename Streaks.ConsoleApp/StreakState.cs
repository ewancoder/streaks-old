namespace Streaks.ConsoleApp
{
    public sealed class StreakState
    {
        public StreakState(int index, string question, int length, bool isDoneToday)
        {
            Index = index;
            Question = question;
            Length = length;
            IsDoneToday = isDoneToday;
        }

        public int Index { get; }
        public string Question { get; }
        public int Length { get; }
        public bool IsDoneToday { get; }
    }
}
