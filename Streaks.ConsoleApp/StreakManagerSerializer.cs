namespace Streaks.ConsoleApp
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    public sealed class StreakManagerSerializer
    {
        private readonly string _fileName;
        private static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof(StreakManager));

        public StreakManagerSerializer(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is empty.");

            _fileName = fileName;
        }

        public StreakManager Load()
        {
            using (var stream = File.OpenRead(_fileName))
                return Serializer.ReadObject(stream) as StreakManager;
        }

        public void Save(StreakManager streakManager)
        {
            using (var stream = File.Create(_fileName))
                Serializer.WriteObject(stream, streakManager);
        }
    }
}
