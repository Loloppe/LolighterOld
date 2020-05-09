using System.Collections.Generic;

namespace Osu2Saber.Model.Json
{
    public class SaberInfo
    {
        List<DifficultyLevel> diffLevels = new List<DifficultyLevel>();

        public string songName;
        public string songSubName;
        public string authorName;
        public double beatsPerMinute;
        public int previewStartTime;
        public int previewDuration;
        public string coverImagePath;
        public string environmentName;
        public IReadOnlyList<DifficultyLevel> difficultyLevels => diffLevels;

        public void AddDifficultyLevels(
            string difficulty, int difficultyRank, string audioPath,
            string jsonPath, int offset)
        {
            var difficultyLevel = new DifficultyLevel
            {
                difficulty = difficulty,
                difficultyRank = difficultyRank,
                audioPath = audioPath,
                jsonPath = jsonPath,
                offset = offset
            };
            diffLevels.Add(difficultyLevel);
        }

        public void ChangeAudioPath(string audioPath)
        {
            foreach (var diffLevel in diffLevels)
            {
                diffLevel.audioPath = audioPath;
            }
        }
    }

    public class DifficultyLevel
    {
        public string difficulty;
        public int difficultyRank;
        public string audioPath;
        public string jsonPath;
        public string madeby;
        public int offset;
    }
}
