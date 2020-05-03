using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Osu2Saber.Model;
using Osu2Saber.Model.Algorithm;

namespace Osu2Saber.ViewModel
{
    class ConfigPanelViewModel : BindableBase
    {
        public double MaximumDifficulty
        {
            set
            {
                Osu2BsConverter.MaximumDifficulty = value;
                RaisePropertyChanged();
            }
            get => Osu2BsConverter.MaximumDifficulty;
        }

        public double MinimumDifficulty
        {
            set
            {
                Osu2BsConverter.MinimumDifficulty = value;
                RaisePropertyChanged();
            }
            get => Osu2BsConverter.MinimumDifficulty;
        }

        public float EnoughIntervalBetweenNotes
        {
            set { ConvertAlgorithm.EnoughIntervalBetweenNotes = value; }
            get => ConvertAlgorithm.EnoughIntervalBetweenNotes;
        }

        public double LightOffset
        {
            set { ConvertAlgorithm.LightOffset = value; }
            get => ConvertAlgorithm.LightOffset;
        }

        public double LimitStacked
        {
            set { ConvertAlgorithm.LimitStacked = value; }
            get => ConvertAlgorithm.LimitStacked;
        }

        public int Mix
        {
            set { ConvertAlgorithm.Mix = value; }
            get => ConvertAlgorithm.Mix;
        }

        public bool AllUpDown
        {
            set { ConvertAlgorithm.AllUpDown = value; }
            get => ConvertAlgorithm.AllUpDown;
        }

        public bool AllBottom
        {
            set { ConvertAlgorithm.AllBottom = value; }
            get => ConvertAlgorithm.AllBottom;
        }

        public bool CreateDouble
        {
            set { ConvertAlgorithm.CreateDouble = value; }
            get => ConvertAlgorithm.CreateDouble;
        }

        public bool GenerateAllStrobe
        {
            set { ConvertAlgorithm.GenerateAllStrobe = value; }
            get => ConvertAlgorithm.GenerateAllStrobe;
        }

        public bool GenerateAudio
        {
            set { Mp3toOggConverter.GenerateAudio = value; }
            get => Mp3toOggConverter.GenerateAudio;
        }

        public string PatternToUse
        {
            set { ConvertAlgorithm.PatternToUse = value; }
            get => ConvertAlgorithm.PatternToUse;
        }

        public bool RandomizeColor
        {
            set { ConvertAlgorithm.RandomizeColor = value; }
            get => ConvertAlgorithm.RandomizeColor;
        }

        public bool OnlyMakeTimingNote
        {
            set { ConvertAlgorithm.OnlyMakeTimingNote = value; }
            get => ConvertAlgorithm.OnlyMakeTimingNote;
        }

        public bool GenerateLights
        {
            set { ConvertAlgorithm.GenerateLight = value; }
            get => ConvertAlgorithm.GenerateLight;
        }

        public bool PreferHarder
        {
            set { Osu2BsConverter.PreferHarder = value; }
            get => Osu2BsConverter.PreferHarder;
        }

        public bool HandleHitSlider
        {
            set { ConvertAlgorithm.IgnoreHitSlider = value; }
            get => ConvertAlgorithm.IgnoreHitSlider;
        }

        public bool NoDirectionAndPlacement
        {
            set { ConvertAlgorithm.NoDirectionAndPlacement = value; }
            get => ConvertAlgorithm.NoDirectionAndPlacement;
        }

        public bool IncludeTaiko
        {
            set { BatchProcessor.IncludeTaiko = value; }
            get => BatchProcessor.IncludeTaiko;
        }

        public bool IncludeCtB
        {
            set { BatchProcessor.IncludeCtB = value; }
            get => BatchProcessor.IncludeCtB;
        }

        public bool IncludeMania
        {
            set { BatchProcessor.IncludeMania = value; }
            get => BatchProcessor.IncludeMania;
        }
    }
}
