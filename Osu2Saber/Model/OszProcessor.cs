using Ionic.Zip;
using Microsoft.Win32;
using osuBMParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Osu2Saber.Model
{
    class OszProcessor
    {
        readonly static string OszDirName = "osz";
        static string workDir;
        public static string WorkDir
        {
            set
            {
                workDir = Path.Combine(value, OszDirName);
                Directory.CreateDirectory(workDir);
            }
            get => workDir;
        }

        readonly string oszPath;

        public string DefaultAudioExtension { set; get; } = "mp3";
        public string OutDir { private set; get; }
        public string[] OsuFiles { private set; get; }
        public string OszName { private set; get; }

        public OszProcessor(string oszPath)
        {
            this.oszPath = oszPath;
            OszName = Path.GetFileNameWithoutExtension(oszPath);
            OutDir = Path.Combine(WorkDir, Path.GetFileNameWithoutExtension(oszPath));
            if (!Directory.Exists(OutDir))
            {
                Directory.CreateDirectory(OutDir);
            }

            Decompress();
            ListOsuFiles();
        }

        void Decompress()
        {
            if (!Directory.Exists(OutDir))
            {
                Directory.CreateDirectory(OutDir);
            }

            using (var archive = ZipFile.Read(oszPath))
            {
                archive.FlattenFoldersOnExtract = true;
                foreach (var entry in archive.Entries)
                {
                    if(!File.Exists(OutDir + "\\" + entry.FileName))
                    {
                        entry.Extract(OutDir, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
        }

        void ListOsuFiles()
        {
            OsuFiles = null;

            while (OsuFiles == null)
            {
                OpenFileDialog open = new OpenFileDialog
                {
                    Filter = "osu|*.osu",
                    Multiselect = false,
                    InitialDirectory = OutDir
                };

                if (open.ShowDialog() == true)
                {
                    OsuFiles = new[] { open.SafeFileName };
                }
            }
        }

        Beatmap LoadOsuFile(string oszName)
        {
            var filePath = Path.Combine(OutDir, oszName);
            Beatmap bm = null;
            try
            {
                bm = new Beatmap(filePath);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Error while parsing " + filePath);
            }
            return bm;
        }

        public IEnumerable<Beatmap> LoadOsuFiles()
        {
            var beatmaps = OsuFiles
            .Select(file => LoadOsuFile(file))
            .Where(map => map != null);
            return beatmaps;
        }
    }
}
