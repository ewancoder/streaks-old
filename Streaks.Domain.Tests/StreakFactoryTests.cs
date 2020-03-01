namespace Streaks.Domain.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public sealed class StreakFactoryTests
    {
        [Theory]
        [MemberData(nameof(StreakLengthData))]
        public void StreakLength(
            IEnumerable<DateTimeOffset> performances,
            DateTimeOffset today,
            TimeSpan dayOffset,
            bool isDoneToday,
            int streakLength)
        {
            var streak = StreakFactory.MakeStreak(performances, today, dayOffset);

            Assert.Equal(isDoneToday, streak.IsDoneToday);
            Assert.Equal(streakLength, streak.Length);
        }

        private static IEnumerable<object[]> StreakLengthData()
        {
            var date = new DateTimeOffset(new DateTime(5, 5, 5, 6, 0, 0), TimeSpan.FromHours(3));

            yield return new object[]
            {
                Enumerable.Empty<DateTimeOffset>(),
                date,
                TimeSpan.Zero,
                false,
                0
            };

            yield return new object[]
            {
                new[] { date.AddDays(-2), date.AddDays(-1) },
                date,
                TimeSpan.Zero,
                false,
                2
            };

            yield return new object[]
            {
                new[] { date.AddDays(-2), date.AddDays(-1), date },
                date,
                TimeSpan.Zero,
                true,
                3
            };

            yield return new object[]
            {
                new[]
                {
                    date.AddDays(-2)
                },
                date,
                TimeSpan.Zero,
                false,
                0
            };

            yield return new object[]
            {
                new[]
                {
                    date.AddDays(-2).AddHours(-6),
                    date.AddDays(-1).AddHours(-6)
                },
                date.AddHours(-6),
                TimeSpan.Zero,
                false,
                2
            };

            yield return new object[]
            {
                new[]
                {
                    date.AddDays(-2).AddHours(-6),
                    date.AddDays(-1).AddHours(-6),
                    date.AddHours(-6)
                },
                date,
                TimeSpan.Zero,
                true,
                3
            };

            yield return new object[]
            {
                new[]
                {
                    date.AddDays(-2).AddHours(17.9),
                    date.AddDays(-1).AddHours(17.9)
                },
                date,
                TimeSpan.Zero,
                false,
                2
            };

            yield return new object[]
            {
                new[]
                {
                    date.AddDays(-2).AddHours(17.9),
                    date.AddDays(-1).AddHours(17.9),
                    date.AddHours(17.9)
                },
                date,
                TimeSpan.Zero,
                true,
                3
            };

            yield return new object[]
            {
                new[]
                {
                    new DateTimeOffset(5, 5, 1, 23, 0, 0, TimeSpan.FromHours(3)),
                    new DateTimeOffset(5, 5, 2, 10, 0, 0, TimeSpan.FromHours(3)),
                    new DateTimeOffset(5, 5, 3, 1, 0, 0, TimeSpan.FromHours(3)),
                    new DateTimeOffset(5, 5, 4, 5, 0, 0, TimeSpan.FromHours(7))
                },
                new DateTimeOffset(5, 5, 4, 10, 0, 0, TimeSpan.FromHours(3)),
                TimeSpan.Zero,
                true,
                4
            };

            yield return new object[]
            {
                new[]
                {
                    new DateTimeOffset(5, 5, 2, 2, 0, 0, TimeSpan.FromHours(3)),
                    new DateTimeOffset(5, 5, 2, 5, 0, 0, TimeSpan.FromHours(3)),
                    new DateTimeOffset(5, 5, 3, 1, 0, 0, TimeSpan.FromHours(3))
                },
                new DateTimeOffset(5, 5, 3, 3, 0, 0, TimeSpan.FromHours(3)),
                TimeSpan.FromHours(4),
                true,
                2
            };
        }
    }
}
