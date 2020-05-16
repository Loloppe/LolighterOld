using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnsetDetection
{
    /// <summary>
    /// Onset Class
    /// </summary>
    public class Onsets
    {
        Vector<float> _activations;
        int _fps;
        bool _online;
        public List<float> Detections;
        public List<float> Amplitudes;
        private float _lastOnset;
        public float LastOnset { get { return _lastOnset; } }

        /// <summary>
        /// Creates a new Onset object instance with the given activations of the ODF (OnsetDetectionFunction).
        /// The activations will be read in from a file
        /// </summary>
        /// <param name="activations">an array containing the activations of the ODF</param>
        /// <param name="fps">frame rate of the activations</param>
        /// <param name="online">work in online mode (i.e. use only past information)</param>
        public Onsets(Vector<float> activations, int fps, bool online = true)
        {
            Initialise(activations, fps, online);
        }

        /// <summary>
        /// Creates a new Onset object instance with the given activations of the ODF (OnsetDetectionFunction).
        /// </summary>
        /// <param name="activationFile">a file containing the activations of the ODF</param>
        /// <param name="fps">frame rate of the activations</param>
        /// <param name="online">work in online mode (i.e. use only past information)</param>>
        public Onsets(string activationFile, int fps, bool online = true)
        {
            //read in activations from file
            Initialise(Load(activationFile), fps, online);
        }

        private void Initialise(Vector<float> activations, int fps, bool online)
        {
            _activations = null; //activations of the ODF
            _fps = fps; //framrate of the activation function
            _online = online; //online peak-picking
            Detections = null; //list of detected onsets (in seconds)
            Amplitudes = null;
            //activations are given as an array
            _activations = activations;
        }

        /// <summary>
        /// Detects the onsets <para />
        /// In online mode, post_avg and post_max are set to 0 <para />
        /// 
        /// Implements the peak-picking method described in: <para />
        /// "Evaluating the Online Capabilities of Onset Detection Methods" <para />
        /// Sebastian Böck, Florian Krebs and Markus Schedl <para />
        /// Proceedings of the 13th International Society for Music Information Retrieval Conference(ISMIR), 2012 <para />
        /// </summary>
        /// <param name="threshold">threshold for peak-picking</param>
        /// <param name="combine">only report 1 onset for N milliseconds</param>
        /// <param name="preAvg">use N milliseconds for moving average</param>
        /// <param name="preMax">use N milliseconds past information for moving maximum</param>
        /// <param name="postAvg">use N milliseconds future information for moving average</param>
        /// <param name="postMax">using N milliseconds future information for moving maximum</param>
        /// <param name="delay">report the onset N milliseconds delayed</param>
        /// <param name="lastOnset">last reported onset relative to delay in milliseconds - can be -ve</param>
        public void Detect(float threshold, float combine=30, float preAvg=100, float preMax=30, float postAvg=30, float postMax=70, float delay=0, float lastOnset=0)
        {
            //online mode?
            if (_online)
            {
                postMax = 0;
                postAvg = 0;
            }
            //convert timing informatino to frames
            preAvg = (int)(Math.Round(_fps * preAvg / 1000f));
            preMax = (int)(Math.Round(_fps * preMax / 1000f));
            postAvg = (int)(Math.Round(_fps * postAvg / 1000f));
            postMax = (int)(Math.Round(_fps * postMax / 1000f));
            //convert to seconds
            combine /= 1000f;
            delay /= 1000f;
            //init detections
            Detections = new List<float>();
            Amplitudes = new List<float>();
            //moving maximum
            var maxLength = (int)(preMax + postMax + 1);
            var maxOrigin = (int)Math.Floor((preMax - postMax) / 2);
            Vector<float> movMax = SciPyCompatibility.MaximumFilter1D(_activations, maxLength, maxOrigin);
            //moving average
            var avgLength = (int)(preAvg + postAvg + 1);
            var avgOrigin = (int)Math.Floor((preAvg - postAvg) / 2);
            Vector<float> movAvg = SciPyCompatibility.UniformFilter1D(_activations, avgLength, avgOrigin);
            //detections are activation equal to the maximum
            var detections = _activations.PointwiseMultiply(_activations.Map2((a, mmax) => (Math.Abs(a - mmax) < float.Epsilon) ? 1 : 0, movMax));
            //detections must be greater or equal than the moving average + threshold
            var mask = detections.Map2((d, mavg) => (d >= (mavg + threshold)) ? 1 : 0, movAvg);
            detections = detections.PointwiseMultiply(detections.Map2((d, mavg) => (d >= (mavg + threshold)) ? 1 : 0, movAvg));
            var detectionAmplitudes = detections.PointwiseMultiply(mask);
            //convert detected onsets to a list of timestamps
            _lastOnset = lastOnset;
            for (int i = 0; i < detections.Count; i++)
            {
                if (detections[i] < float.Epsilon) continue;
                var onset = i / (float)_fps + delay;
                //only report an onset if the last N milliseconds none was reported
                if (onset > _lastOnset + combine)
                {
                    Detections.Add(onset);
                    Amplitudes.Add(detectionAmplitudes[i]);
                    //save the last reported onset
                    _lastOnset = onset;
                }
            }
        }

        public void Write(string filename)
        {
            throw new System.NotImplementedException();
        }

        public void Save(string filename)
        {
            throw new System.NotImplementedException();
        }

        public Vector<float> Load(string filename)
        {
            throw new System.NotImplementedException();
        }
    }
}