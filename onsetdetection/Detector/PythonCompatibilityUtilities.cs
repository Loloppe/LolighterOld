using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace OnsetDetection
{
    static class PythonUtilities
    {
        public static ArraySegment<T> Slice<T>(T[] array, int startIndex, int endIndex)
        {
            return new ArraySegment<T>(array, startIndex, endIndex - startIndex);
        }

        public static void SetArraySegment<T>(ArraySegment<T> segment, T[] values)
        {
            for (int i = segment.Offset; i < segment.Offset + segment.Count; i++)
            {
                segment.Array[i] = values[i - segment.Offset];
            }
        }
    }

    //following class originally from https://gist.github.com/wcharczuk/3948606
    static class NumpyCompatibility
    {
        public static IEnumerable<float> Arange(float start, int count)
        {
            return Enumerable.Range((int)start, count).Select(v => (float)v);
        }

        public static IEnumerable<float> Power(IEnumerable<float> exponents, float baseValue = 10.0f)
        {
            return exponents.Select(v => (float)Math.Pow(baseValue, v));
        }

        public static IEnumerable<float> LinSpace(float start, float stop, int num, bool endpoint = true)
        {
            var result = new List<float>();
            if (num <= 0)
            {
                return result;
            }

            if (endpoint)
            {
                if (num == 1)
                {
                    return new List<float>() { start };
                }

                var step = (stop - start) / (num - 1.0f);
                result = Arange(0, num).Select(v => (v * step) + start).ToList();
            }
            else
            {
                var step = (stop - start) / num;
                result = Arange(0, num).Select(v => (v * step) + start).ToList();
            }

            return result;
        }

        public static IEnumerable<float> LogSpace(float start, float stop, int num, bool endpoint = true, float numericBase = 10.0f)
        {
            var y = LinSpace(start, stop, num: num, endpoint: endpoint);
            return Power(y, numericBase);
        }

        public static Vector<float> FFTShift(Vector<float> x)
        {
            var y = x;
            var p2 = (int)Math.Floor((x.Count + 1) / 2.0f);
            var indicies = Enumerable.Range(p2, x.Count - p2).ToList();
            indicies.AddRange(Enumerable.Range(0, p2));
            for (int i = 0; i < indicies.Count; i++)
            {
                y[i] = x[indicies[i]];
            }
            return y;
        }
    }

    public static class SciPyCompatibility
    {
        public static Matrix<float> UniformFilter1D(Matrix<float> input, int size, int origin)
        {
            var o = size / 2;
            o += origin;
            return input.MapIndexed((r, c, f) => UniformFilter1DHelper(input, r, c, size, o));
        }

        private static float UniformFilter1DHelper(Matrix<float> input, int r, int c, int size, int o)
        {
            float s = 0;
            for (int i = c - o; i < c - o + size; i++)
            {
                if (i < 0 || i >= input.ColumnCount) s += 0;
                else s += input[r, i];
            }
            return s / size;
        }

        public static Matrix<float> MaximumFilter1D(Matrix<float> input, int size, int origin)
        {
            var o = size / 2;
            o += origin;
            return input.MapIndexed((r, c, f) => MaximumFilter1DHelper(input, r, c, size, o));
        }

        private static float MaximumFilter1DHelper(Matrix<float> input, int r, int c, int size, int o)
        {
            float s = 0;
            for (int i = c - o; i < c - o + size; i++)
            {
                if (!(i < 0 || i >= input.ColumnCount) && input[r, i] > s) s = input[r, i];
            }
            return s;
        }

        public static Vector<float> UniformFilter1D(Vector<float> input, int size, int origin)
        {
            var o = size / 2;
            o += origin;
            return input.MapIndexed((c, f) => UniformFilter1DHelper(input, c, size, o));
        }

        private static float UniformFilter1DHelper(Vector<float> input, int c, int size, int o)
        {
            float s = 0;
            for (int i = c - o; i < c - o + size; i++)
            {
                if (i < 0 || i >= input.Count) s += 0;
                else s += input[i];
            }
            return s / size;
        }

        public static Vector<float> MaximumFilter1D(Vector<float> input, int size, int origin)
        {
            var o = size / 2;
            o += origin;
            return input.MapIndexed((c, f) => MaximumFilter1DHelper(input, c, size, o));
        }

        private static float MaximumFilter1DHelper(Vector<float> input, int c, int size, int o)
        {
            float s = 0;
            for (int i = c - o; i < c - o + size; i++)
            {
                if (!(i < 0 || i >= input.Count) && input[i] > s) s = input[i];
            }
            return s;
        }
    }
}
