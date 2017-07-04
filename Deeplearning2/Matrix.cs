using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deeplearning2
{
    public class Matrix
    {
        public double[,] matrix { get; private set; }
        public int width, height;

        public Matrix(int width, int height)
        {
            matrix = new double[width, height];
            this.width = width;
            this.height = height;
        }

        public static Matrix operator *(Matrix matrix, double scalar)
        {
            return ApplyFunc(matrix, x => x * scalar);
        }

        public Matrix Multiply(Matrix secondMatrix)
        {
            var newMatrix = new Matrix(width, secondMatrix.height);
            double[,] new_matrix = new double[width, secondMatrix.height];
            for (int x = 0; x < newMatrix.width; x++)
            {
                for (int y = 0; y < newMatrix.height; y++)
                {
                    for (int k = 0; k < height; k++)
                    {
                        new_matrix[x, y] += matrix[x, k] * secondMatrix.matrix[k, y];
                    }
                }
            }
            newMatrix.matrix = new_matrix;
            return newMatrix;
        }

        public Matrix Calculate(Matrix vector)
        {
            return (Matrix) Multiply(vector);
        }

        public static Matrix operator -(Matrix A, Matrix B)
        {
            if (A.width != B.width)
                throw new Exception("Matrices dont match");
            double[,] newmatrix = new double[A.width, A.height];
            for (int x = 0; x < A.width; x++)
            {
                for (int y = 0; y < A.height; y++)
                {
                    newmatrix[x, y] = A.matrix[x, y] - B.matrix[x, y];
                }
            }
            var Matrix = new Matrix(A.width, A.height);
            Matrix.matrix = newmatrix;
            return Matrix;
        }

        public static Matrix ApplyFunc(Matrix A, Func<double, double> func)
        {
            double[,] newmatrix = new double[A.width, A.height];
            for (int x = 0; x < A.width; x++)
            {
                for (int y = 0; y < A.height; y++)
                {
                    newmatrix[x, y] = func(A.matrix[x, y]);
                }
            }
            var Matrix = new Matrix(A.width, A.height);
            Matrix.matrix = newmatrix;
            return Matrix;
        }

        public static Matrix HadamardMultiply(Matrix A, Matrix B)
        {
            if(A.width != B.width || A.height != B.height)
                throw new Exception("Matrices dont match");
            double[,] newmatrix = new double[A.width, A.height];
            for (int x = 0; x < A.width; x++)
            {
                for (int y = 0; y < A.height; y++)
                {
                    newmatrix[x, y] = A.matrix[x, y] * B.matrix[x, y];
                }
            }
            var Matrix = new Matrix(A.width, A.height);
            Matrix.matrix = newmatrix;
            return Matrix;
        }

        public Matrix Transpose()
        {
            var newMatrix = new Matrix(height, width);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newMatrix.matrix[y, x] = matrix[x, y];
                }
            }
            return newMatrix;
        }
    }
}
