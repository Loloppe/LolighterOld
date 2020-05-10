using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using osuBMParser;
using Newtonsoft.Json;
using System.IO;
using Osu2Saber.Model.Algorithm;
using Osu2Saber.Model.Json;

namespace Osu2Saber.Model
{
    /// <summary>
    /// This class generates info.json and [difficulty].json files from an.osu file.
    /// </summary>
    public class Osu2BsConverter
    {
        static readonly string MapDirName = "output";
        static Formatting formatting = Formatting.Indented;
        static string workDir;
        public static string WorkDir
        {
            set
            {
                workDir = Path.Combine(value, MapDirName);
                //Directory.CreateDirectory(workDir);
            }
            get => workDir;
        }
        public static double MinimumDifficulty { set; get; } = 0;
        public static double MaximumDifficulty { set; get; } = 10;
        public static bool PreferHarder { set; get; } = true;

        const string InfoFileName = "info.json";
        const int MaxNumOfBeatmap = 5;

        SaberInfo info;
        List<Beatmap> beatmaps = new List<Beatmap>();

        public string OrgDir { private set; get; }
        public string OutDir { private set; get; }
        public string AudioPath { private set; get; }
        public string ImagePath { private set; get; }
        public static bool GenerateAudio { get; internal set; }

        public static SaberBeatmap map = new SaberBeatmap();

        public Osu2BsConverter(string orgDir, string outputDirName)
        {
            OrgDir = orgDir;
            OutDir = Path.Combine(workDir, outputDirName);
            //Directory.CreateDirectory(OutDir);
        }

        public void AddBeatmap(Beatmap org)
        {
            beatmaps.Add(org);
        }

        public void ProcessAll()
        {
            SortAndPickBeatmaps();
            InitializeInfo(beatmaps[0]);
            for (var i=0; i<beatmaps.Count; i++)
            {
                ConvertBeatmap(i);
            }
        }

        public void GenerateInfoFile(string audioFileName)
        {
            info.ChangeAudioPath(audioFileName);
            var infoJson = JsonConvert.SerializeObject(info, formatting);
            var infoPath = Path.Combine(OutDir, InfoFileName);
            using (var sw = new StreamWriter(infoPath, false, Encoding.UTF8))
            {
                sw.Write(infoJson);
            }
        }

        void SortAndPickBeatmaps()
        {
            beatmaps = beatmaps.OrderBy(beatmap => beatmap.HitObjects.Count).ToList();
            if (beatmaps.Count <= MaxNumOfBeatmap) return;

            if (PreferHarder)
                beatmaps = beatmaps.Skip(beatmaps.Count - MaxNumOfBeatmap).ToList();
            else
                beatmaps = beatmaps.Take(MaxNumOfBeatmap).ToList();
        }

        void ConvertBeatmap(int index)
        {
            var org = beatmaps[index];
            map = GenerateMap(org);
        }

        void InitializeInfo(Beatmap org)
        {
            if(org.ImageFileName == null)
            {
                org.ImageFileName = "cover.jpg";
            }

            info = new SaberInfo
            {
                songName = org.Title,
                songSubName = org.Artist,
                authorName = "",
                beatsPerMinute = CalcOriginalBPM(org),
                previewStartTime = org.PreviewTime / 1000,
                previewDuration = 10,
                coverImagePath = Path.ChangeExtension(org.ImageFileName, ".jpg"),
                environmentName = "NiceEnvironment",
            };

            AudioPath = Path.Combine(OrgDir, org.AudioFileName);
            ImagePath = Path.Combine(OrgDir, org.ImageFileName);
        }

        SaberBeatmap GenerateMap(Beatmap org)
        {
            var map = new SaberBeatmap()
            {
                origin = org.Version,
                _version = "1.5.0",
                _beatsPerMinute = CalcOriginalBPM(org),
                _beatsPerBar = 16,
                _noteJumpSpeed = 20,
                _shuffle = 0,
                _shufflePeriod = 0.5,
            };
            var ca = new ConvertAlgorithm(org, map);
            ca.Convert();

            map._events = ca.Events;
            map._obstacles = ca.Obstacles;
            map._notes = ca.Notes;
            return map;
        }

        double CalcOriginalBPM(Beatmap org)
        {
            var tp = org.TimingPoints[0];
            var mpb = tp.MsPerBeat;
            return Math.Round(1000.0 / mpb * 60, 3);
        }
    }
}
