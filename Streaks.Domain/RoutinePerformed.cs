namespace Streaks.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract(Name = "routine")]
    public sealed class Routine
    {
        [DataMember(Name = "performances", Order = 1)]
        private readonly List<DateTimeOffset> _performances;

        [DataMember(Name = "question", Order = 2)]
        private string _question;

        public Routine(string question)
        {
            _performances = new List<DateTimeOffset>();
            _question = question;
        }

        public string Question => _question;

        public void EditQuestion(string question)
        {
            _question = question;
        }

        public void Yes(DateTimeOffset now)
        {
            _performances.Add(now);
        }

        public void CancelLast()
        {
            if (_performances.Count > 0)
                _performances.RemoveAt(_performances.Count - 1);
        }

        public Streak CalculateStreak(DateTimeOffset now, TimeSpan dayOffset)
        {
            return StreakFactory.MakeStreak(_performances, now, dayOffset);
        }
    }

    public static class StreakFactory
    {
        public static Streak MakeStreak(
            IEnumerable<DateTimeOffset> performances,
            DateTimeOffset today,
            TimeSpan dayOffset)
        {
            var lastDate = default(DateTimeOffset);
            var streakLength = 1;

            foreach (var date in performances)
            {
                if (lastDate == default(DateTimeOffset))
                {
                    lastDate = date;
                    continue;
                }

                if (date.UtcDateTime - lastDate.UtcDateTime > TimeSpan.FromDays(1))
                {
                    streakLength = 1;
                    lastDate = date;
                    continue;
                }

                if ((today - dayOffset).Date == (lastDate.UtcDateTime + today.Offset - dayOffset).Date)
                    continue;

                streakLength++;
                lastDate = date;
            }

            if (today.UtcDateTime - lastDate.UtcDateTime > TimeSpan.FromDays(1))
                return new Streak(0, false);

            var isDoneToday = (today - dayOffset).Date == (lastDate.UtcDateTime + today.Offset - dayOffset).Date;

            return new Streak(streakLength, isDoneToday);
        }
    }

    public sealed class Streak
    {
        public Streak(int length, bool isDoneToday)
        {
            Length = length;
            IsDoneToday = isDoneToday;
        }

        public int Length { get; }
        public bool IsDoneToday { get; }
    }
}
