
using Osu2Saber.Model;
using Osu2Saber.Model.Algorithm;

namespace Osu2Saber.ViewModel
{
    class ConfigPanelViewModel
    {
        public double MaximumDifficulty
        {
            set
            {
                Osu2BsConverter.MaximumDifficulty = value;
            }
            get => Osu2BsConverter.MaximumDifficulty;
        }

        public double MinimumDifficulty
        {
            set
            {
                Osu2BsConverter.MinimumDifficulty = value;
            }
            get => Osu2BsConverter.MinimumDifficulty;
        }

        public double EnoughIntervalBetweenNotes
        {
            set { ConvertAlgorithm.EnoughIntervalBetweenNotes = value; }
            get => ConvertAlgorithm.EnoughIntervalBetweenNotes;
        }

        public double ParitySpeed
        {
            set { ConvertAlgorithm.ParitySpeed = value; }
            get => ConvertAlgorithm.ParitySpeed;
        }

        public double GallopSpeed
        {
            set { ConvertAlgorithm.GallopSpeed = value; }
            get => ConvertAlgorithm.GallopSpeed;
        }

        public double SlowSpeed
        {
            set { ConvertAlgorithm.SlowSpeed = value; }
            get => ConvertAlgorithm.SlowSpeed;
        }

        public bool AllUpDown
        {
            set { ConvertAlgorithm.AllUpDown = value; }
            get => ConvertAlgorithm.AllUpDown;
        }
        
        public bool DoubleHitboxFix
        {
            set { ConvertAlgorithm.DoubleHitboxFix = value; }
            get => ConvertAlgorithm.DoubleHitboxFix;
        }

        public bool AllTopUp
        {
            set { ConvertAlgorithm.AllTopUp = value; }
            get => ConvertAlgorithm.AllTopUp;
        }

        public bool AllowOneHanded
        {
            set { ConvertAlgorithm.AllowOneHanded = value; }
            get => ConvertAlgorithm.AllowOneHanded;
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

        public bool GenerateGallops
        {
            set { ConvertAlgorithm.GenerateGallops = value; }
            get => ConvertAlgorithm.GenerateGallops;
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
