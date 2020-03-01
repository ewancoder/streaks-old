namespace Streaks.ConsoleApp
{
    using Streaks.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Name = "streakManager")]
    public sealed class StreakManager
    {
        [DataMember(Name = "streaks", Order = 1)]
        private readonly List<Streak> _streaks = new List<Streak>();

        [DataMember(Name = "endOfDayOffset", Order = 2)]
        private TimeSpan _endOfDayOffset;

        private IEnumerable<Routine> Routines => _streaks.Select(s => s.ConvertToRoutine());

        public TimeSpan EndOfDayOffset
        {
            get => _endOfDayOffset;
            set
            {
                if (value >= TimeSpan.FromDays(1) || value <= TimeSpan.FromDays(-1))
                    throw new InvalidOperationException("End of day offset should be in a range of 1 day.");

                _endOfDayOffset = value;
            }
        }

        public IEnumerable<StreakState> GetState()
        {
            return GetStateNew();
        }

        public IEnumerable<StreakState> GetStateOld()
        {
            var now = GetNormalizedNow();
            var index = 1;

            foreach (var streak in _streaks)
            {
                yield return new StreakState(index, streak.Question, streak.CalculateStreakLength(now), streak.IsDoneToday(now));
                index++;
            }
        }

        public IEnumerable<StreakState> GetStateNew()
        {
            var now = GetNormalizedNow();
            var index = 1;

            foreach (var routine in Routines)
            {
                // HACK for now:
                now = now.Date;

                var streak = routine.CalculateStreak(now, _endOfDayOffset);

                yield return new StreakState(index, routine.Question, streak.Length, streak.IsDoneToday);
                index++;
            }
        }

        public int GetStreaksCount()
        {
            return _streaks.Count;
        }

        // The "now" that comes here is already normalized to 0-24h relative time.
        public TimeSpan GetTimeLeftUntilTheEndOfDay()
        {
            var now = GetNormalizedNow();

            return TimeSpan.FromDays(1) - new TimeSpan(now.Hour, now.Minute, now.Second);
        }

        public void Yes(int index)
        {
            var now = GetNormalizedNow();

            _streaks[index - 1].Yes(now);
        }

        public void No(int index)
        {
            var now = GetNormalizedNow();

            _streaks[index - 1].No(now);
        }

        public void UpdateQuestion(int index, string question)
        {
            _streaks[index - 1].Question = question;
        }

        public void AddStreak(string question)
        {
            _streaks.Add(new Streak(question));
        }

        public void ClearStreaks()
        {
            _streaks.Clear();
        }

        // TODO: Use time service.
        private DateTimeOffset GetNormalizedNow()
        {
            return DateTimeOffset.Now - _endOfDayOffset;
        }
    }
}
