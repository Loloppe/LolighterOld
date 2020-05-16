using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Storage;

namespace OnsetDetection
{
    public class MemoryAllocator
    {
        private List<DenseColumnMajorMatrixStorage<float>> _floatMatrixStoragePool;
        private List<DenseColumnMajorMatrixStorage<Complex32>> _complex32MatrixStoragePool;
        private List<DenseVectorStorage<float>> _floatVectorStoragePool;
        private List<DenseVectorStorage<Complex32>> _complex32VectorStoragePool;

        private List<DenseColumnMajorMatrixStorage<float>> _floatMatrixStoragePoolMaster;
        private List<DenseColumnMajorMatrixStorage<Complex32>> _complex32MatrixStoragePoolMaster;
        private List<DenseVectorStorage<float>> _floatVectorStoragePoolMaster;
        private List<DenseVectorStorage<Complex32>> _complex32VectorStoragePoolMaster;

        private object _lock = new object();

        public MemoryAllocator()
        {
            _floatMatrixStoragePool = new List<DenseColumnMajorMatrixStorage<float>>();
            _complex32MatrixStoragePool = new List<DenseColumnMajorMatrixStorage<Complex32>>();
            _floatVectorStoragePool = new List<DenseVectorStorage<float>>();
            _complex32VectorStoragePool = new List<DenseVectorStorage<Complex32>>();

            _floatMatrixStoragePoolMaster = new List<DenseColumnMajorMatrixStorage<float>>();
            _complex32MatrixStoragePoolMaster = new List<DenseColumnMajorMatrixStorage<Complex32>>();
            _floatVectorStoragePoolMaster = new List<DenseVectorStorage<float>>();
            _complex32VectorStoragePoolMaster = new List<DenseVectorStorage<Complex32>>();
        }

        public string GetStatus()
        {
            return string.Format("fM - {0}, fV - {1}, cM - {2}, cV - {3}", 
                _floatMatrixStoragePoolMaster.Count, _floatVectorStoragePoolMaster.Count, 
                _complex32MatrixStoragePoolMaster.Count, _complex32VectorStoragePoolMaster.Count);
        }

        public void Reset()
        {
            _complex32MatrixStoragePool.Clear(); _complex32MatrixStoragePool.AddRange(_complex32MatrixStoragePoolMaster);
            _complex32VectorStoragePool.Clear(); _complex32VectorStoragePool.AddRange(_complex32VectorStoragePoolMaster);

            _floatMatrixStoragePool.Clear(); _floatMatrixStoragePool.AddRange(_floatMatrixStoragePoolMaster);
            _floatVectorStoragePool.Clear(); _floatVectorStoragePool.AddRange(_floatVectorStoragePoolMaster);
        }

        public Matrix<float> GetFloatMatrix(int rows, int cols)
        {
            lock (_lock)
            {
                var storage = _floatMatrixStoragePool.Find(s => s.RowCount == rows && s.ColumnCount == cols);
                if (storage == null)
                {
                    storage = DenseColumnMajorMatrixStorage<float>.OfValue(rows, cols, 0.0f);
                    _floatMatrixStoragePoolMaster.Add(storage);
                }
                else
                {
                    _floatMatrixStoragePool.Remove(storage);
                    storage.Clear();
                }
                return Matrix<float>.Build.Dense(storage);

            }
        }

        public void ReturnFloatMatrixStorage(DenseColumnMajorMatrixStorage<float> storage)
        {
            lock (_lock)
            {
                if (!_floatMatrixStoragePool.Contains(storage))
                    _floatMatrixStoragePool.Add(storage);

            }
        }

        public Matrix<Complex32> GetComplex32Matrix(int rows, int cols)
        {
            lock (_lock)
            {
                var storage = _complex32MatrixStoragePool.Find(s => s.RowCount == rows && s.ColumnCount == cols);
                if (storage == null)
                {
                    storage = DenseColumnMajorMatrixStorage<Complex32>.OfValue(rows, cols, Complex32.Zero);
                    _complex32MatrixStoragePoolMaster.Add(storage);
                }
                else
                {
                    _complex32MatrixStoragePool.Remove(storage);
                    storage.Clear();
                }
                return Matrix<Complex32>.Build.Dense(storage);

            }
        }

        public void ReturnComplex32MatrixStorage(DenseColumnMajorMatrixStorage<Complex32> storage)
        {
            lock (_lock)
            {
                if (!_complex32MatrixStoragePool.Contains(storage))
                    _complex32MatrixStoragePool.Add(storage);

            }
        }

        public Vector<float> GetFloatVector(int length)
        {
            lock (_lock)
            {
                var storage = _floatVectorStoragePool.Find(s => s.Length == length);
                if (storage == null)
                {
                    storage = DenseVectorStorage<float>.OfValue(length, 0.0f);
                    _floatVectorStoragePoolMaster.Add(storage);
                }
                else
                {
                    _floatVectorStoragePool.Remove(storage);
                    storage.Clear();
                }
                return Vector<float>.Build.Dense(storage);

            }
        }

        public void ReturnFloatVectorStorage(DenseVectorStorage<float> storage)
        {
            lock (_lock)
            {
                if (!_floatVectorStoragePool.Contains(storage))
                    _floatVectorStoragePool.Add(storage);

            }
        }

        public Vector<Complex32> GetComplex32Vector(int length)
        {
            lock (_lock)
            {
                var storage = _complex32VectorStoragePool.Find(s => s.Length == length);
                if (storage == null)
                {
                    storage = DenseVectorStorage<Complex32>.OfValue(length, Complex32.Zero);
                    _complex32VectorStoragePoolMaster.Add(storage);
                }
                else
                {
                    _complex32VectorStoragePool.Remove(storage);
                    storage.Clear();
                }
                return Vector<Complex32>.Build.Dense(storage);
            }
        }

        public void ReturnComplex32VectorStorage(DenseVectorStorage<Complex32> storage)
        {
            lock (_lock)
            {
                if (!_complex32VectorStoragePool.Contains(storage))
                    _complex32VectorStoragePool.Add(storage);
            }
        }
    }
}
