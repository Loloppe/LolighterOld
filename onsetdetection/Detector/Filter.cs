using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using System.Diagnostics;

namespace OnsetDetection
{
    /// <summary>
    /// Filter class
    /// </summary>
    public class Filter
    {
        int _fs;
        public Matrix<float> Filterbank;
        MemoryAllocator _allocator;

        /// <summary>
        /// Creates a new Filter object instance
        /// </summary>
        /// <param name="ffts">number of FFT coefficients</param>
        /// <param name="fs">sample rate of the audio file</param>
        /// <param name="bands">number of filter bands</param>
        /// <param name="fmin">the minimum frequency [in Hz]</param>
        /// <param name="fmax">the maximum frequency [in Hz]</param>
        /// <param name="equal">normalize each band to equal energy</param>
        public Filter(int ffts, int fs, MemoryAllocator allocator, int bands=12, float fmin=27.5f, float fmax=16000f, bool equal=false)
        {
            _allocator = allocator;

            //Samplerate
            _fs = fs;
            //reduce fmax if necessary
            if (fmax > _fs / 2) fmax = _fs / 2;
            //get a list of frequencies
            var frequencies = Frequencies(bands, fmin, fmax);
            //conversion factor for mapping of frequencies to spectogram bins
            var factor = (fs / 2f) / ffts;
            //map the frequencies to the spectogram bins
            int[] sbins = new int[frequencies.Length];
            for (int i = 0; i < frequencies.Length; i++)
            {
                sbins[i] = (int)Math.Round(frequencies[i] / factor);
            }
            //only keep unique bins
            sbins = sbins.Distinct().ToArray();
            //filter out all frequencies outside the valid range
            sbins = sbins.Where(i => i < ffts).ToArray();
            //number of bands
            bands = sbins.Length - 2;
            Debug.Assert(bands >= 3, "cannot create filterbank with less than 3 frequencies");
            //init the filter matrix with size: ffts x filter bands
            Filterbank = _allocator.GetFloatMatrix(ffts, bands);
            //Filterbank = Matrix<float>.Build.Dense(ffts, bands);
            //process all bands
            foreach (var band in Enumerable.Range(0,bands))
            {
                //edge and center frequencies
                var start = sbins[band];
                var mid = sbins[band + 1];
                var stop = sbins[band + 2];
                //create a triangular filter
                Filterbank.SetColumn(band, start, stop - start, Vector<float>.Build.DenseOfArray(Triang(start, mid, stop, equal)));
            }
        }

        /// <summary>
        /// Returns a list of frequencies aligned on a logarithmic scale
        /// Using 12 bands per octave and a=440 corresponding to the MIDI notes.
        /// </summary>
        /// <param name="bands">number of filter bands per octave</param>
        /// <param name="fmin">the minimum frequency [in Hz]</param>
        /// <param name="fmax">the maximum frequency [in Hz]</param>
        /// <param name="a">frequency of A0 [in Hz]</param>
        /// <returns>a list of frequencies</returns>
        public static float[] Frequencies(int bands, float fmin, float fmax, float a=440)
        {
            //factor 2 frequencies are apart
            var factor = (float)Math.Pow(2.0, 1.0 / bands);
            //start with A0
            var freq = a;
            var frequencies = new List<float> { freq };
            //go upwards till max
            while (freq <= fmax)
            {
                //multiply once more, since the included frequency is a frequency
                //which is only used as the right corner of a (triangular) filter
                freq *= factor;
                frequencies.Add(freq);
            }
            //restart with a and go downwards till fmin
            freq = a;
            while (freq >= fmin)
            {
                //divide once more, since the included frequency is a frequency
                //which is only used as the left corner of a (triangular) filter
                freq /= factor;
                frequencies.Add(freq);
            }
            //sort frequencies
            frequencies.Sort();
            //return the list
            return frequencies.ToArray();
        }

        /// <summary>
        /// Calculates a triangular window of the given size.
        /// </summary>
        /// <param name="start">starting bin (with value 0, included in the returned filter)</param>
        /// <param name="mid">center bin (of height 1, unless norm is True)</param>
        /// <param name="stop">end bin (with value 0, not included in the returned filter)</param>
        /// <param name="equal">normalize the area of the filter to 1 [default=False]</param>
        /// <returns>a triangular shaped filter</returns>
        public static float[] Triang(int start, int mid, int stop, bool equal=false)
        {
            //height of the filter
            var height = 1.0f;
            //normalize the height
            if (equal) height = 2f / (stop - start);
            //init the filter
            float[] triang_filter = new float[stop - start];
            //rising edge
            PythonUtilities.SetArraySegment(PythonUtilities.Slice(triang_filter, 0, mid - start), NumpyCompatibility.LinSpace(0, height, (mid- start), false).ToArray());
            //falling edge
            PythonUtilities.SetArraySegment(PythonUtilities.Slice(triang_filter, mid - start, triang_filter.Length), NumpyCompatibility.LinSpace(height, 0, (stop - mid), false).ToArray());
            //return
            return triang_filter;
        }

        public void Cleanup()
        {
            _allocator.ReturnFloatMatrixStorage((MathNet.Numerics.LinearAlgebra.Storage.DenseColumnMajorMatrixStorage<float>)Filterbank.Storage);
        }
    }
}