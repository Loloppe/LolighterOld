using CSCore;
using CSCore.Codecs;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnsetDetection
{
    public class OnsetDetector
    {
        private List<float> _onsets;
        private List<float> _amplitudes;
        private DetectorOptions _options;
        private object _lock;
        private int _sliceCount;
        private int _completed;

        public IProgress<string> ProgressReporter
        {
            get { return _progressReporter ?? new Progress<string>(); }
            set { _progressReporter = value; }
        }
        IProgress<string> _progressReporter;

        public OnsetDetector(DetectorOptions options, IProgress<string> progress)
        {
            _onsets = new List<float>();
            _amplitudes = new List<float>();
            _options = options;
            _lock = new object();
            ProgressReporter = progress;
        }

        public List<Onset> Detect(string audioFile)
        {
            var ss = CodecFactory.Instance.GetCodec(audioFile).ToSampleSource();
            return Detect(ss);
        } 

        public List<Onset> Detect(ISampleSource audio)
        {
            _onsets.Clear();
            _completed = 0;
            _sliceCount = 0;
            _onsets = new List<float>();
            _amplitudes = new List<float>();
            var onsets = new List<Onset>();

            //init detection specific variables
            int sliceSampleSize = (int)Math.Ceiling(_options.SliceLength * audio.WaveFormat.SampleRate); //the size of each slice's sample
            int slicePaddingSize = (int)Math.Ceiling(_options.SlicePaddingLength * audio.WaveFormat.SampleRate);
            _sliceCount = (int)Math.Ceiling((float)audio.Length/audio.WaveFormat.Channels / sliceSampleSize); //the number of slices needed
            var samples = (int)audio.Length / audio.WaveFormat.Channels;

            //init parallel specific variables
            var pOptions = new ParallelOptions();
            if (_options.MaxDegreeOfParallelism != -1) pOptions.MaxDegreeOfParallelism = _options.MaxDegreeOfParallelism;
            ParallelLoopState loopState;

            List<Wav> wavSlices = new List<Wav>();
            for (int i = 0; i < _sliceCount; i++)
            {
                int baseStart = i * sliceSampleSize;
                int adjustedStart = (baseStart - sliceSampleSize > 0) ? baseStart - slicePaddingSize : 0;
                int count = (sliceSampleSize + slicePaddingSize + baseStart > samples) ? samples - adjustedStart : sliceSampleSize + (baseStart - adjustedStart) + slicePaddingSize;
                float delay = (float)adjustedStart / audio.WaveFormat.SampleRate;
                float[] buffer = new float[count * audio.WaveFormat.Channels];
                audio.SetPosition(TimeConverter.SampleSourceTimeConverter.ToTimeSpan(audio.WaveFormat, adjustedStart * audio.WaveFormat.Channels));
                audio.Read(buffer, 0, count * audio.WaveFormat.Channels);
                wavSlices.Add(new Wav(buffer, audio.WaveFormat.SampleRate, count, audio.WaveFormat.Channels) {
                    Delay = delay,
                    Padding = ((delay > 0) ? slicePaddingSize : 0) / audio.WaveFormat.SampleRate
                });
            }

            int bucketSize = 5;
            int bucketcount = (int)Math.Ceiling((double)wavSlices.Count / bucketSize);
            MemoryAllocator _allocator = new MemoryAllocator();

            for (int i = 0; i < bucketcount; i++)
            {
                _allocator.Reset();
                int count = bucketSize;
                if ((i + 1) * bucketSize > wavSlices.Count) count = wavSlices.Count - i * bucketSize;

                if (count < 0) continue;

                List<Wav> parallel = wavSlices.GetRange(i * bucketSize, count);
                var ploopResult = Parallel.ForEach(parallel, pOptions, (w, state) => GetOnsets(w, _allocator));
                if (!ploopResult.IsCompleted) throw new Exception();
            }

            onsets = _onsets.Zip(_amplitudes, (onset, amplitude) => new Onset { OnsetTime = onset, OnsetAmplitude = amplitude }).ToList();
            onsets = onsets.OrderBy(f => f.OnsetTime).ToList();

            float prev = 0;
            float combine = 0.03f;
            var ret = new List<Onset>();
            for (int i = 0; i < onsets.Count; i++)
            {
                if (onsets[i].OnsetTime - prev < _options.MinimumTimeDelta / 1000.0f)
                    continue;
                prev = onsets[i].OnsetTime;
                ret.Add(onsets[i]);
            }
            return ret;
        }

        private void GetOnsets(Wav w, MemoryAllocator allocator)
        {
            //construct the spectrogram
            var s = new Spectrogram(w, allocator, _options.WindowSize, _options.FPS, _options.Online, NeedPhaseInformation(_options.DetectionFunction));

            //perform adaptive whitening
            if (_options.AdaptiveWhitening) s.AW(_options.AWFloor, _options.AWRelax);

            //construct the filterbank
            var filt = new Filter(_options.WindowSize / 2, w.Samplerate, allocator);

            //filter the spectrogram
            s.Filter(filt.Filterbank);

            //take the log of the spectrogram
            if (_options.Log) s.Log(_options.LogMultiplier, _options.LogAdd);

            //calculate the activations
            var sodf = new SpectralODF(s, allocator);
            var act = GetActivations(sodf, _options.DetectionFunction);

            //detect the onsets
            var o = new Onsets(act, _options.FPS);
            o.Detect(_options.ActivationThreshold, _options.MinimumTimeDelta,  delay: w.Delay * 1000);
            var count = o.Detections.Count(f => f < (w.Delay + w.Padding));

            //add the onsets to the collection
            lock (_lock)
            {
                _onsets.AddRange(o.Detections.Skip(count));
                _amplitudes.AddRange(o.Amplitudes.Skip(count));
            }

            _completed++;
            ProgressReporter.Report(String.Format("{0}%", Math.Round((((float)_completed / _sliceCount))*100f)));

            //cleanup
            s.Cleanup();
            filt.Cleanup();
        }

        private Vector<float> GetActivations(SpectralODF sODF, Detectors detectionFunction)
        {
            Vector<float> activations;
            switch (detectionFunction)
            {
                case Detectors.HFC:
                    activations = sODF.HFC();
                    break;
                case Detectors.SD:
                    activations = sODF.SD();
                    break;
                case Detectors.SF:
                    activations = sODF.SF();
                    break;
                case Detectors.MKL:
                    activations = sODF.MKL();
                    break;
                case Detectors.PD:
                    activations = sODF.PD();
                    break;
                case Detectors.WPD:
                    activations = sODF.WPD();
                    break;
                case Detectors.NWPD:
                    activations = sODF.NWPD();
                    break;
                case Detectors.CD:
                    activations = sODF.CD();
                    break;
                case Detectors.RCD:
                    activations = sODF.RCD();
                    break;
                default:
                    throw new Exception("Unsupported detection function");
                    break;
            }

            return activations;
        }

        private bool NeedPhaseInformation(Detectors detectionFunction)
        {
            return new Detectors[] { Detectors.PD, Detectors.WPD, Detectors.NWPD, Detectors.CD, Detectors.RCD }.Contains(detectionFunction);
        }
    }

    public struct DetectorOptions
    {
        /// <summary>
        /// Slice the audio up into segments of this length for parallelism. Default is 10.0f <para />
        /// Given in seconds
        /// </summary>
        public float SliceLength;

        /// <summary>
        /// Padding to add to either end of a slice to ensure no beats are missed. Default is 0.01f <para /> 
        /// Given in seconds
        /// </summary>
        public float SlicePaddingLength;

        /// <summary>
        /// The max degree of parallelism to use. Default is -1 - scheduler decides
        /// </summary>
        public int MaxDegreeOfParallelism;

        /// <summary>
        /// The activation threshold to use for the detection. Default is 5f
        /// </summary>
        public float ActivationThreshold;

        /// <summary>
        /// The minimum time that must occur between successive onsets. Default is 30f <para />
        /// Given in milliseconds.
        /// </summary>
        public float MinimumTimeDelta;

        /// <summary>
        /// The size of the window in samples. Default is 2048
        /// </summary>
        public int WindowSize;

        /// <summary>
        /// The frames-per-second of the detector.  Default is 200
        /// </summary>
        public int FPS;

        /// <summary>
        /// Whether to use only past information or not. Default is true
        /// </summary>
        public bool Online;

        /// <summary>
        /// The onset detection function to use. Default is Detectors.SF
        /// </summary>
        public Detectors DetectionFunction;

        /// <summary>
        /// Whether to take the log of the spectrogram. Default is true
        /// </summary>
        public bool Log;

        /// <summary>
        /// Multiplier before taking the log. Default is 1
        /// </summary>
        public float LogMultiplier;

        /// <summary>
        /// Value added before taking the log. Default is 1
        /// </summary>
        public float LogAdd;

        /// <summary>
        /// Whether to apply adaptive whitening. Default is false
        /// </summary>
        public bool AdaptiveWhitening;

        /// <summary>
        /// Floor value for adaptive whitening. Default is 5.0f
        /// </summary>
        public float AWFloor;

        /// <summary>
        /// Relaxation time for adaptive whitening. Default is 10.0f
        /// </summary>
        public float AWRelax;

        public static DetectorOptions Default
        {
            get
            {
                return new DetectorOptions
                {
                    SliceLength = 10.0f,
                    SlicePaddingLength = 0.01f,
                    MaxDegreeOfParallelism = -1,
                    ActivationThreshold = 5f,
                    MinimumTimeDelta = 30f,
                    WindowSize = 2048,
                    FPS = 200,
                    Online = true,
                    DetectionFunction = Detectors.SF,
                    Log = true,
                    LogMultiplier = 1,
                    LogAdd = 1,
                    AdaptiveWhitening = false,
                    AWFloor = 5.0f,
                    AWRelax = 10.0f
                };
            }
        }
    }

    public enum Detectors
    {
        /// <summary>High Frequency Content</summary>
        HFC,
        /// <summary>Spectral Diff</summary>
        SD,
        /// <summary>Spectral Flux</summary>
        SF,
        /// <summary>Modified Kullback-Leibler</summary>
        MKL,
        /// <summary>Phase Deviation</summary>
        PD,
        /// <summary>Weighted Phase Deviation</summary>
        WPD,
        /// <summary>Normalized Weighted Phase Deviation</summary>
        NWPD,
        /// <summary>Complex Domain</summary>
        CD,
        /// <summary>Rectified Complex Domain</summary>
        RCD
    }

    public struct Onset
    {
        public float OnsetTime;
        public float OnsetAmplitude;

        public override string ToString()
        {
            return string.Format("{0},{1}", OnsetTime, OnsetAmplitude);
        }
    }

    public class OnsetHelper
    {
        public Spectrogram Spectrogram;
        public Filter Filter;
        public SpectralODF SprectralODF;
        public Onsets Onsets;
    }
}
