using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSCore;
using CSCore.Codecs;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

namespace OnsetDetection
{
    /// <summary>
    /// Wav Class is a simple wrapper around cscore
    /// </summary>
    public class Wav
    {
        public int Samplerate { get; private set; }
        public int Samples { get; private set; }
        public int Channels { get; private set; }
        public Matrix<float> Audio { get; private set; }
        public float Delay { get; set; }
        public float Padding { get; set; }

        /// <summary>
        /// Creates a new Wav object instance of the given file
        /// </summary>
        /// <param name="filename">name of the .wav file</param>
        public Wav(string filename)
        {
            //read in the audio
            var ss = CodecFactory.Instance.GetCodec(filename).ToSampleSource();
            Initialise(ss);
        }

        /// <summary>
        /// Creates a new Wav object instance of the given sample source
        /// </summary>
        /// <param name="sampleSource">sample source to use</param>
        public Wav(ISampleSource sampleSource)
        {
            Initialise(sampleSource);
        }

        /// <summary>
        /// Creates a new Wav object of the given audio matrix and wave format information
        /// </summary>
        /// <param name="audio">audio data [of dimensions (channel count, sample count)] </param>
        /// <param name="samplerate"></param>
        /// <param name="samples"></param>
        /// <param name="channels"></param>
        public Wav(Matrix<float> audio, int samplerate, int samples, int channels)
        {
            Audio = audio;
            Initialise(samplerate, samples, channels);
        }

        /// <summary>
        /// Creates a new Wav object of the given audio matrix and wave format information
        /// </summary>
        /// <param name="audio">audio data [of dimensions (channel count, sample count)] </param>
        /// <param name="samplerate"></param>
        /// <param name="samples"></param>
        /// <param name="channels"></param>
        public Wav(float[] audio, int samplerate, int samples, int channels)
        {
            Initialise(samplerate, samples, channels);
            LoadAudioData(audio);
            DownMix();
        }


        private void Initialise(int samplerate, int samples, int channels)
        {
            Samplerate = samplerate;
            Samples = samples;
            Channels = channels;
            Delay = 0.0f;
        }

        private void Initialise(ISampleSource sampleSource)
        {
            Initialise(sampleSource.WaveFormat.SampleRate, (int)sampleSource.Length / sampleSource.WaveFormat.Channels, sampleSource.WaveFormat.Channels);
            float[] buffer = new float[sampleSource.Length];
            sampleSource.Read(buffer, 0, (int)sampleSource.Length);
            LoadAudioData(buffer);

        }

        private void LoadAudioData(float[] buffer)
        {
            //load the channel data
            Audio = DenseMatrix.Create(Channels, buffer.Length / Channels, 0);
            for (int i = 0; i < Audio.ColumnCount; i++)
            {
                for (int j = 0; j < Audio.RowCount; j++)
                {
                    Audio[j, i] = buffer[i * Channels + j];
                }
            }
        }

        /// <summary>
        /// Attenuate the audio signal
        /// </summary>
        /// <param name="attenuation">attenuation level given in dB</param>
        public void Attenuate(float attenuation)
        {
            Audio = Audio.Divide((float)Math.Pow(Math.Sqrt(10), attenuation / 10));
        }

        /// <summary>
        /// Down-mix the signal to mono
        /// </summary>
        public void DownMix()
        {
            if (Channels > 1)
                Audio = Matrix<float>.Build.DenseOfRowVectors(Audio.ColumnSums().Divide(Channels));
        }

        /// <summary>
        /// Normalize the audio signal
        /// </summary>
        public void Normalize()
        {
            Audio = Audio.Divide(Audio.ToArray().Cast<float>().Max());
        }
    }
}