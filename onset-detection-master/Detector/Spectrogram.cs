using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex32;
using MathNet.Numerics;

namespace OnsetDetection
{
    /// <summary>
    /// Spectrogram Class
    /// </summary>
    public class Spectrogram
    {
        MemoryAllocator _allocator;
        Wav _wav;
        int _fps;
        public float HopSize;
        int _frames;
        int _ffts;
        public int Bins;
        Matrix<Complex32> _STFT;
        public Matrix<float> Phase;
        public Matrix<float> Spec;
        public Vector<float> Window;

        /// <summary>
        /// Creates a new Spectrogram object instance and performs a STFT on the given audio
        /// </summary>
        /// <param name="wav">a Wav object</param>
        /// <param name="windowSize">is the size for the window in samples</param>
        /// <param name="fps">is the desired frame rate</param>
        /// <param name="online">work in online mode (i.e. use only past audio information)</param>
        /// <param name="phase">include phase information</param>
        public Spectrogram(Wav wav, MemoryAllocator allocator, int windowSize=2048, int fps=200, bool online=true, bool phase=true)
        {
            _allocator = allocator;
            //init some variables
            _wav = wav;
            _fps = fps;
            //derive some variables
            HopSize = _wav.Samplerate / (float)_fps; //use floats so that seeking works properly
            _frames = (int)(_wav.Samples / HopSize);
            _ffts = windowSize / 2;
            Bins = windowSize / 2; //initial number equal to ffts, can change if filters are used

            //init STFT matrix
            _STFT = _allocator.GetComplex32Matrix(_frames, _ffts);
            //_STFT = DenseMatrix.Create(_frames, _ffts, Complex32.Zero);

            //create windowing function
            var cArray = wav.Audio.ToRowArrays()[0];

            var values = MathNet.Numerics.Window.Hann(windowSize).Select(d => (float)d).ToArray();
            Window = _allocator.GetFloatVector(values.Length);
            Window.SetValues(values);

            //Window = Vector<float>.Build.DenseOfArray(MathNet.Numerics.Window.Hann(windowSize).Select(d => (float)d).ToArray());

            //step through all frames
            System.Numerics.Complex[] result = new System.Numerics.Complex[Window.Count];
            foreach (var frame in Enumerable.Range(0, _frames))
            {
                int seek;
                Vector<float> signal;
                //seek to the right position in the audio signal
                if (online)
                    //step back a complete windowSize after moving forward 1 hopSize
                    //so that the current position is at the stop of the window
                    seek = (int)((frame + 1) * HopSize - windowSize);
                else
                    //step back half of the windowSize so that the frame represents the centre of the window
                    seek = (int)(frame * HopSize - windowSize / 2);
                //read in the right portion of the audio
                if (seek >= _wav.Samples)
                    //stop of file reached
                    break;
                else if (seek + windowSize > _wav.Samples)
                {
                    //stop behind the actual audio stop, append zeros accordingly
                    int zeroAmount = seek + windowSize - _wav.Samples;
                    //var zeros = Vector<float>.Build.Dense(zeroAmount, 0);

                    var t = PythonUtilities.Slice<float>(cArray, seek, cArray.Length).ToArray();

                    //t.AddRange(zeros.ToList());

                    signal = _allocator.GetFloatVector(t.Length + zeroAmount);
                    for (int i = 0; i < t.Length; i++)
                    {
                        signal[i] = t[i];
                    }
                    //signal.SetValues(t);
                    //signal = Vector<float>.Build.DenseOfEnumerable(t);
                }
                else if (seek < 0)
                {
                    //start before actual audio start, pad with zeros accordingly
                    int zeroAmount = -seek;
                    var zeros = Vector<float>.Build.Dense(zeroAmount, 0).ToList();

                    var t = PythonUtilities.Slice<float>(cArray, 0, seek + windowSize).ToArray();
                    zeros.AddRange(t);

                    signal = _allocator.GetFloatVector(t.Length + zeroAmount);
                    signal.SetValues(zeros.ToArray());
                    //signal = Vector<float>.Build.DenseOfEnumerable(zeros);
                }
                else
                {
                    //normal read operation
                    var slice = PythonUtilities.Slice<float>(cArray, seek, seek + windowSize).ToArray();
                    signal = _allocator.GetFloatVector(slice.Length);
                    signal.SetValues(slice);

                    //signal = Vector<float>.Build.DenseOfEnumerable(PythonUtilities.Slice<float>(cArray, seek, seek + windowSize));
                }
                //multiply the signal with the window function
                signal = signal.PointwiseMultiply(Window);
                //only shift and perform complex DFT if needed
                if (phase)
                {
                    //circular shift the signal (needed for correct phase)
                    signal = NumpyCompatibility.FFTShift(signal);
                }
                //perform DFT
                //sanity check
                Debug.Assert(result.Length == signal.Count);
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = signal[i];
                }
                MathNet.Numerics.IntegralTransforms.Fourier.BluesteinForward(result, MathNet.Numerics.IntegralTransforms.FourierOptions.NoScaling);
                _STFT.SetRow(frame, result.Select(r => new Complex32((float)r.Real, (float)r.Imaginary)).Take(_ffts).ToArray());
                //var _newSTFTRow = result.Select(r => new Complex32((float)r.Real, (float)r.Imaginary)).Take(_ffts).ToArray();
                //_STFT.SetRow(frame, _newSTFTRow);
                //next frame
                _allocator.ReturnFloatVectorStorage((MathNet.Numerics.LinearAlgebra.Storage.DenseVectorStorage<float>)signal.Storage);
            }
            //magnitude spectrogram

            Spec = _allocator.GetFloatMatrix(_STFT.RowCount, _STFT.ColumnCount);
            if (phase)
                Phase = _allocator.GetFloatMatrix(_STFT.RowCount, _STFT.ColumnCount);
            for (int i = 0; i < Spec.RowCount; i++)
            {
                for (int j = 0; j < Spec.ColumnCount; j++)
                {
                    Spec.At(i, j, _STFT.At(i, j).Magnitude);
                    if (phase)
                        Phase.At(i, j, _STFT.At(i, j).Phase);
                }
            }
            //Spec = _STFT.Map(c => (float)c.Magnitude);

            //phase
            //if (phase)
            //{
            //    var imag = _STFT.Map(c => (float)c.Imaginary);
            //    var real = _STFT.Map(c => (float)c.Real);
            //    Phase = real.Map2((r, i) => (float)Math.Atan2(i,r), imag);
            //}
        }

        /// <summary>
        /// Perform adaptive whitening on the magnitude spectrogram
        /// </summary>
        /// <param name="floor">floor value</param>
        /// <param name="relaxation">relaxation time in seconds</param>
        /// "Adaptive Whitening For Improved Real-time Audio Onset Detection"
        /// Dan Stowell and Mark Plumbley
        /// Proceedings of the International Computer Music Conference(ICMC), 2007
        public void AW(float floor=5, float relaxation=10)
        {
            var memCoeff = (float)Math.Pow(10.0, (-6 * relaxation / _fps));
            var P = _allocator.GetFloatMatrix(Spec.RowCount, Spec.ColumnCount);
            //var P = Matrix<float>.Build.SameAs(Spec);

            //iterate over all frames
            Vector<float> spec_floor = _allocator.GetFloatVector(Spec.ColumnCount);
            //var spec_floor = Vector<float>.Build.Dense(Spec.ColumnCount);
            foreach (var f in Enumerable.Range(0, _frames))
            {
                spec_floor.Clear();
                for (int i = 0; i < Spec.ColumnCount; i++)
                {
                    spec_floor[i] = Math.Max(Spec[f, i], floor);
                }
                //var spec_floor = Math.Max(Spec.ToRowArrays()[f].ToList().Max(), floor);
                if (f > 0)
                    for (int i = 0; i < P.ColumnCount; i++)
                    {
                        P[f, i] = Math.Max(spec_floor[i], memCoeff * P[f - 1, i]);
                    }
                else
                    P.SetRow(f, spec_floor);
            }
            //adjust spec
            Spec = Spec.PointwiseDivide(P);

            //cleanup
            _allocator.ReturnFloatMatrixStorage((MathNet.Numerics.LinearAlgebra.Storage.DenseColumnMajorMatrixStorage<float>)P.Storage);
            _allocator.ReturnFloatVectorStorage((MathNet.Numerics.LinearAlgebra.Storage.DenseVectorStorage<float>)spec_floor.Storage);
        }

        /// <summary>
        /// Filter the magnitude spectrogram with a filterbank
        /// If no filter is given a standard one will be created
        /// </summary>
        /// <param name="Filterbank">Filter object which includes the filterbank</param>
        public void Filter(Matrix<float> filterbank = null)
        {
            Filter f = null;
            if (filterbank == null)
            {
                //construct a standard filterbank
                f = new Filter(_ffts, _wav.Samplerate, _allocator);
                filterbank = f.Filterbank;
            }
            //filter the magnitude spectrogram with the filterbank
            Spec = Spec.Multiply(filterbank);
            //adjust the number of bins
            Bins = Spec.ColumnCount;

            //cleanup
            if (f != null) f.Cleanup();
        }

        /// <summary>
        /// Take the logarithm of the magnitude spectrogram
        /// </summary>
        /// <param name="mul">multiply the magnitude spectrogram with given value</param>
        /// <param name="add">add the given value to the magnitude spectrogram</param>
        public void Log(float mul=20, float add=1)
        {
            if (add <= 0) throw new Exception("a positive value must be added before taking the logarithm");
            Spec = Spec.Map(f => (float)Math.Log10(mul * f + add), Zeros.Include);
        }

        public void Cleanup()
        {
            _allocator.ReturnComplex32MatrixStorage((MathNet.Numerics.LinearAlgebra.Storage.DenseColumnMajorMatrixStorage<Complex32>)_STFT.Storage);
            if (Phase != null) _allocator.ReturnFloatMatrixStorage((MathNet.Numerics.LinearAlgebra.Storage.DenseColumnMajorMatrixStorage<float>)Phase.Storage);
            _allocator.ReturnFloatMatrixStorage((MathNet.Numerics.LinearAlgebra.Storage.DenseColumnMajorMatrixStorage<float>)Spec.Storage);
            _allocator.ReturnFloatVectorStorage((MathNet.Numerics.LinearAlgebra.Storage.DenseVectorStorage<float>)Window.Storage);

        }
    }
}