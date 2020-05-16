using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;
using System.Numerics;

namespace OnsetDetection
{
    /// <summary>
    /// The SpectralODF class implements most of the common onset detection function based on
    /// the magnitued or phase information of a spectrogram
    /// </summary>
    public class SpectralODF
    {
        Spectrogram _s;
        int _diffFrames;
        MemoryAllocator _allocator;

        /// <summary>
        /// Creates a new ODF object instance
        /// </summary>
        /// <param name="spectogram">the spectrogram on which the detection functions operate</param>
        /// <param name="ratio">calculate the difference to the frame which has the given magnitude ratio</param>
        /// <param name="frames">calculate the difference to the N-th previous frame</param>
        public SpectralODF(Spectrogram spectogram, MemoryAllocator allocator, float ratio=0.22f, int frames=0)
        {
            _s = spectogram;
            _allocator = allocator;
            //determine the number of diff frames
            if (frames == 0)
            {
                //get the first sample with a higher magnitude than given ratio
                var sample = _s.Window.Find(f => f > ratio).Item1;
                var diff_samples = _s.Window.Count / 2 - sample;
                //convert to frames
                frames = (int)Math.Round(diff_samples / _s.HopSize);
            }
            //set the minimum to 1
            if (frames < 1) frames = 1;
            _diffFrames = frames;
        }

        /// <summary>
        /// Wrap the phase informatino to the range -π...π
        /// </summary>
        /// <param name="angle">angle to wrap</param>
        /// <returns></returns>
        public static float WrapToPi(float angle)
        {
            return (float)((angle + Math.PI) % (2.0 * Math.PI) - Math.PI);
        }

        public static Matrix<float> WrapToPi(Matrix<float> angle)
        {
            return angle.Map(f => WrapToPi(f));
        }

        /// <summary>
        /// Calculates the difference on the magnitude spectrogram
        /// </summary>
        /// <param name="spec">the magnitude spectrogram</param>
        /// <param name="pos">only keep positive values</param>
        /// <param name="diffFrames">calculate the difference to the N-th previous frame</param>
        public Matrix<float> Diff(Matrix<float> spec, bool pos=false, int diffFrames=0)
        {
            var diff = _allocator.GetFloatMatrix(spec.RowCount, spec.ColumnCount);
            //var diff = Matrix<float>.Build.SameAs(spec);
            if (diffFrames == 0) diffFrames = _diffFrames;
            //calculate the diff
            var subMatrix = spec.SubMatrix(diffFrames, spec.RowCount - diffFrames, 0, spec.ColumnCount).Subtract(spec.SubMatrix(0, spec.RowCount - diffFrames, 0, spec.ColumnCount));
            diff.SetSubMatrix(diffFrames, 0, subMatrix);
            if (pos)
                diff = diff.PointwiseMultiply(diff.Map(f => (float)((f > 0) ? 1 : -1)));
            return diff;
        }

        /// <summary>
        /// High Frequency Content
        ///         "Computer Modeling of Sound for Transformation and Synthesis of Musical Signals"
        ///         Paul Masri
        ///         PhD thesis, University of Bristol, 1996
        /// </summary>
        public Vector<float> HFC()
        {
            //HFC weights the magnitude spectrogram by the bin number, thus emphasising high frequencies
            var weightMatrix = Matrix<float>.Build.DenseOfRows(Enumerable.Repeat(Enumerable.Range(0, _s.Bins).Cast<float>(), _s.Spec.RowCount));
            var tmp = _s.Spec.PointwiseMultiply(weightMatrix);
            var ret = tmp.RowSums().Divide(tmp.ColumnCount);
            //return _s.Spec.PointwiseMultiply(Matrix<float>.Build.DenseOfRows(Enumerable.Repeat(Enumerable.Range(0, _s.Bins).Cast<float>(), _s.Spec.RowCount))).RowSums().Divide(_s.Spec.ColumnCount);
            return ret;
        }

        /// <summary>
        /// Spectral Diff
        /// "A hybrid approach to musical note onset detection"
        /// Chris Duxbury, Mark Sandler and Matthew Davis
        /// Proceedings of the 5th International Conference on Digital Audio Effects(DAFx-02), 2002.
        /// </summary>
        public Vector<float> SD()
        {
            //Spectral diff is the sum of all squared positive 1st order differences
            var diff = Diff(_s.Spec, true);
            diff = diff.PointwisePower(2);
            return diff.RowSums();
        }

        /// <summary>
        /// Spectral Flux
        /// "Computer Modeling of Sound for Transformation and Synthesis of Musical Signals"
        /// Paul Masri
        /// PhD thesis, University of Bristol, 1996
        /// </summary>
        public Vector<float> SF()
        {
            //Spectral flux is the sum of all positive 1st order differences
            return Diff(_s.Spec, true).RowSums();
        }

        /// <summary>
        /// Modified Kullback-Leibler
        /// we use the implenmentation presented in:
        /// "Automatic Annotation of Musical Audio for Interactive Applications"
        /// Paul Brossier
        /// PhD thesis, Queen Mary University of London, 2006
        /// 
        /// instead of the original work:
        /// "Onset Detection in Musical Audio Signals"
        /// Stephen Hainsworth and Malcolm Macleod
        /// Proceedings of the International Computer Music Conference(ICMC), 2003
        /// </summary>
        /// <param name="epsilon">add epsilon to avoid division by 0</param>
        public Vector<float> MKL(float epsilon=0.000001f)
        {
            if (epsilon > 0) throw new ArgumentOutOfRangeException("epsilon", "epsilon must be a positive value");
            var mkl = Matrix<float>.Build.SameAs<float>(_s.Spec);
            var set = _s.Spec.SubMatrix(1, _s.Spec.RowCount - 1, 0, _s.Spec.ColumnCount).PointwiseDivide(_s.Spec.SubMatrix(0, _s.Spec.RowCount - 1, 0, _s.Spec.ColumnCount).Map(f => f + epsilon));
            mkl.SetSubMatrix(1, 0, set);
            //note: the original MKL uses sum instead of mean, but the range of the mean is much more suitable
            return mkl.Map(f => (float)Math.Log(f + 1)).RowSums().Divide(mkl.ColumnCount);
        }

        /// <summary>
        /// Helper method used by PD() and WPD()
        /// </summary>
        public Matrix<float> _PD()
        {
            var pd = Matrix<float>.Build.SameAs(_s.Phase);
            //instantaneous frequency is given by the first difference ψ′(n, k) = ψ(n, k) − ψ(n − 1, k)
            //change in instantaneous frequency is given by the second order difference ψ′′(n, k) = ψ′(n, k) − ψ′(n − 1, k)
            var tmp = _s.Phase.SubMatrix(2, _s.Phase.RowCount - 2, 0, _s.Phase.ColumnCount) - (2 * _s.Phase.SubMatrix(1, _s.Phase.RowCount - 2, 0, _s.Phase.ColumnCount)) + _s.Phase.SubMatrix(0, _s.Phase.RowCount - 2, 0, _s.Phase.ColumnCount);
            pd.SetSubMatrix(2, 0, tmp);
            //map to the range -pi..pi
            return WrapToPi(pd);
        }

        /// <summary>
        /// Phase Deviation
        /// "On the use of phase and energy for musical onset detection in the complex domain"
        /// Juan Pablo Bello, Chris Duxbury, Matthew Davies and Mark Sandler
        /// IEEE Signal Processing Letters, Volume 11, Number 6, 2004
        /// </summary>
        public Vector<float> PD()
        {
            //take the mean of the absolute changes in instantaneous frequency
            var tmp = _PD();
            return tmp.RowAbsoluteSums().Divide(tmp.ColumnCount);
        }

        /// <summary>
        /// Weighted Phase Deviation
        /// "Onset Detection Revisited"
        /// Simon Dixon
        /// Proceedings of the 9th International Conference on Digital Audio Effects(DAFx), 2006
        /// </summary>
        public Vector<float> WPD()
        {
            //make sure the spectrogram is not filtered before
            Debug.Assert(_s.Phase.ColumnCount == _s.Spec.ColumnCount && _s.Phase.RowCount == _s.Spec.RowCount);
            //wpd = spec * pd
            var tmp = _PD().PointwiseMultiply(_s.Spec);
            return tmp.RowAbsoluteSums().Divide(tmp.ColumnCount);
        }

        /// <summary>
        /// Normalized Weighted Phase Deviation
        /// "Onset Detection Revisited"
        /// Simon Dixon
        /// Proceedings of the 9th International Conference on Digital Audio Effects(DAFx), 2006
        /// </summary>
        /// <param name="epsilon">add epsilon to avoid division by 0</param>
        public Vector<float> NWPD(float epsilon=0.00001f)
        {
            if (epsilon > 0) throw new ArgumentOutOfRangeException("epsilon", "epsilon must be a positive value");
            //normalize WPD by the sum of the spectrogram (add a small amount so that we don't divide by 0)
            var tmp = _s.Spec.RowSums().Divide(_s.Spec.ColumnCount) + epsilon;
            return WPD().PointwiseDivide(tmp);
        }

        /// <summary>
        /// Helper method used by CD() & RCD()
        /// we use the simple implementation presented in:
        /// "Onset Detection Revisited"
        /// Simon Dixon
        /// Proceedings of the 9th International Conference on Digital Audio Effects(DAFx), 2006
        /// </summary>
        /// <returns></returns>
        public Matrix<Complex> _CD()
        {
            //make sure the spectrogram is not filtered before
            Debug.Assert(_s.Phase.ColumnCount == _s.Spec.ColumnCount && _s.Phase.RowCount == _s.Spec.RowCount);
            //expected spectrogram
            var cdTarget = Matrix<float>.Build.SameAs(_s.Phase);

            //TODO: Implement this method!!

            //assume constant phase change
            cdTarget.SetSubMatrix(1, 0, (2 * _s.Phase.SubMatrix(1, _s.Phase.RowCount - 1, 0, _s.Phase.ColumnCount)) - _s.Phase.SubMatrix(0, _s.Phase.RowCount - 1, 0, _s.Phase.ColumnCount));
            //add magnitude
            var tmp = _s.Spec.Map(f => (Complex)f) * cdTarget.Map(f => Complex.Exp(Complex.ImaginaryOne + f));
            //complex spectrogram
            //note: construct new instead of using self.stft, because pre-processing could have been applied
            var cd = _s.Spec.Map(f => (Complex)f) * _s.Phase.Map(f => Complex.Exp(Complex.ImaginaryOne + f));
            //subtract the target values
            cd.SetSubMatrix(1, 0, cd.SubMatrix(1, cd.RowCount - 1, 0, cd.ColumnCount) - tmp.SubMatrix(0, tmp.RowCount - 1, 0, tmp.ColumnCount));
            return cd;
        }

        /// <summary>
        /// Complex Domain
        /// "On the use of phase and energy for musical onset detection in the complex domain"
        /// Juan Pablo Bello, Chris Duxbury, Matthew Davies and Mark Sandler
        /// IEEE Signal Processing Letters, Volume 11, Number 6, 2004
        /// </summary>
        public Vector<float> CD()
        {
            //take the sum of the absolute changes

            return _CD().RowAbsoluteSums().Map(c => (float)c.Magnitude);
        }

        /// <summary>
        /// Rectified Complex Domain
        /// "Onset Detection Revisited"
        /// Simon Dixon
        /// Proceedings of the 9th International Conference on Digital Audio Effects(DAFx), 2006
        /// </summary>
        public Vector<float> RCD()
        {
            //rectified complex domain
            var rcd = _CD();
            //only keep values where the magnitude rises
            var mult = _s.Spec.SubMatrix(1, _s.Spec.RowCount - 1, 0, _s.Spec.ColumnCount).Subtract(_s.Spec.SubMatrix(0, _s.Spec.RowCount - 1, 0, _s.Spec.ColumnCount)).Map<Complex>(f => (f > 0) ? 1 : 0);
            rcd.SetSubMatrix(1, 0, rcd.SubMatrix(1, rcd.RowCount - 1, 0, rcd.ColumnCount).PointwiseMultiply(mult));
            return rcd.RowAbsoluteSums().Map(c => (float)c.Magnitude);
        }
    }
}