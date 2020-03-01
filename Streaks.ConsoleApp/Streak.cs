namespace Streaks.ConsoleApp
{
    using Streaks.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Name = "streak")]
    public sealed class Streak
    {
        [DataMember(Name = "days", Order = 2)]
        private readonly HashSet<DateTime> _utcDays = new HashSet<DateTime>();

        [DataMember(Name = "question", Order = 1)]
        private string _question;

        public Streak(string question)
        {
            Question = question;
        }

        public string Question
        {
            get => _question;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Question can't be empty.");

                _question = value;
            }
        }

        public bool IsDoneToday(DateTimeOffset today)
        {
            var utcDay = GetUtcDay(today);

            if (!_utcDays.Contains(utcDay))
                return false;

            return true;
        }

        public void Yes(DateTimeOffset today)
        {
            var utcDay = GetUtcDay(today);

            _utcDays.Add(utcDay);
        }

        public void No(DateTimeOffset today)
        {
            var utcDay = GetUtcDay(today);

            _utcDays.Remove(utcDay);
        }

        // TODO: Refactor, improve and clean this method after covering it with tests.
        public int CalculateStreakLength(DateTimeOffset today)
        {
            var lastDay = default(DateTime);
            var processingDay = default(DateTime);
            var streakLength = 1;

            foreach (var day in _utcDays.Select(d => d + today.Offset))
            {
                if (processingDay == default(DateTime))
                {
                    processingDay = day;
                    lastDay = day;
                    continue;
                }

                if (day - processingDay == TimeSpan.FromDays(streakLength))
                {
                    streakLength++;
                }
                else
                {
                    processingDay = day;
                    streakLength = 1;
                }

                lastDay = day;
            }

            if (lastDay != GetUtcDay(today) + today.Offset && lastDay != GetUtcDay(today).AddDays(-1) + today.Offset)
                return 0;

            return streakLength;
        }

        private static DateTime GetUtcDay(DateTimeOffset date)
        {
            return date.UtcDateTime.Date;
        }

        public Routine ConvertToRoutine()
        {
            var routine = new Routine(_question);

            foreach (var day in _utcDays)
            {
                var d = day.ToLocalTime();
                routine.Yes(new DateTimeOffset(day.ToLocalTime(), TimeSpan.FromHours(3)));
            }

            return routine;
        }
    }
}
