using System;
using System.Net;
using System.Runtime.Remoting.Channels;

namespace MatrixLibrary
{
    public class Matrix
    {

        private double[,] _matrix;
        private static Random random = new Random();
        
        /// <summary>
        /// Instantiate a new matrix lxc
        /// </summary>
        /// <param name="l">number of rows</param>
        /// <param name="c">number of column</param>
        public Matrix(int l, int c)
        {
            _matrix = new double[l, c];
            // l => Height
            // c => Width
        }
        
        /// <summary>
        /// Instanciate a new matrix lxc with a value
        /// </summary>
        /// <param name="l">number of rows</param>
        /// <param name="c">number of column</param>
        /// <param name="val">default value in matrix cell</param>
        public Matrix(int l, int c, long val)
        {
            _matrix = new double[l, c];
            
            for (int i = 0; i < Height(); i++)
            {
                for (int j = 0; j < Width(); j++)
                {
                    _matrix[i, j] = val;
                }
            }
            
        }

        /// <summary>
        /// Fill with random value between 'start' and 'end'
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void Randomize(int start, int end)
        {
            for (int l = 0; l < Height(); l++)
            {
                for (int c = 0; c < Width(); c++)
                {
                    _matrix[l, c] = random.Next(start, end);
                }
            }
        }

        /// <summary>
        /// Add another matrix to the instance
        /// </summary>
        /// <param name="m">Matrix to add</param>
        /// <exception cref="MatrixException">Dimension not egual</exception>
        public void Add(Matrix m)
        {
            if (Width() != m.Width() || Height() != m.Height())
                throw new MatrixException("Dimension not egual");

            for (int c = 0; c < Width(); c++)
            {
                for (int l = 0; l < Height(); l++)
                {
                    _matrix[l, c] += m.get(l, c);
                }
            }
        }

        /// <summary>
        /// Multiply by a scaller
        /// </summary>
        /// <param name="sc"></param>
        public void Scaller(double sc)
        {
            for (int i = 0; i < Height(); i++)
            {
                for (int j = 0; j < Width(); j++)
                {
                    _matrix[i, j] *= sc;
                }
            }
        }

        /// <summary>
        /// Map a function to each element in the matrix
        /// </summary>
        /// <param name="func"></param>
        public void Map(Func<double, double> func)
        {
            for (int i = 0; i < Height(); i++)
            {
                for (int j = 0; j < Width(); j++)
                {
                    _matrix[i, j] = func(_matrix[i, j]);
                }
            }
        }

        public int Width()
        {
            return _matrix.GetLength(1);
        }
        
        public int Height()
        {
            return _matrix.GetLength(0);
        }
        
        public double get(int l, int c)
        {
            if (c < 0 || l < 0 || c >= Width() || l >= Height())
                throw new MatrixException("Can't access to position outside the matrix (" + c + ", " + l + ") ]" + Height() + ", " + Width()  +"[");
            return _matrix[l, c];
        }

        public void set(int l, int c, double d)
        {
            if (c < 0 || l < 0 || c >= Width() || l >= Height())
                throw new MatrixException("Can't set position outside the matrix");
            _matrix[l, c] = d;
        }
        
        /*
         * Static function
         */

        public static Matrix Randomized(int l, int c, int rStart, int rEnd)
        {
            Matrix m = new Matrix(l, c);
            
            for (int i = 0; i < m.Height(); i++)
            {
                for (int j = 0; j < m.Width(); j++)
                {
                    m.set(i, j, random.Next(rStart, rEnd));
                }
            }

            return m;
        }
        
        public static Matrix Add(Matrix a, Matrix b)
        {
            if (a.Width() != b.Width() || a.Height() != b.Height())
                throw new MatrixException("Dimension not egual");

            Matrix m = new Matrix(a.Height(), a.Width());
            
            for (int l = 0; l < a.Height(); l++)
            {
                for (int c = 0; c < a.Width(); c++)
                {
                    m.set(l, c, a.get(l, c) + b.get(l, c));
                }
            }

            return m;
        }

        public static Matrix Sub(Matrix a, Matrix b)
        {
            if (a.Width() != b.Width() || a.Height() != b.Height())
                throw new MatrixException("Dimension not egual");

            Matrix m = new Matrix(a.Height(), a.Width());
            
            for (int l = 0; l < a.Height(); l++)
            {
                for (int c = 0; c < a.Width(); c++)
                {
                    m.set(l, c, a.get(l, c) - b.get(l, c));
                }
            }

            return m;
        }
        
        public static Matrix Mult(Matrix a, Matrix b)
        {
            if (a.Width() != b.Height())
                throw new MatrixException("Width is not egual to the Height (" + a.Width() + ", " + b.Height() + ")");

            Matrix tmp = new Matrix(a.Height(), b.Width());
            
            for (int l = 0; l < tmp.Height(); l++)
            {
                for (int c = 0; c < tmp.Width(); c++)
                {
                    for (int k = 0; k < b.Height(); k++)
                    {
                        tmp.set(l, c, tmp.get(l, c) + a.get(l, k) * b.get(k, c));
                    }
                }
            }

            return tmp;
        }
        
        public static Matrix Map(Matrix m, Func<double, double> func)
        {
            Matrix result = new Matrix(m.Height(), m.Width());
            
            for (int i = 0; i < m.Height(); i++)
            {
                for (int j = 0; j < m.Width(); j++)
                {
                    result.set(i, j, func(m.get(i, j)));
                }
            }

            return result;
        }

        public static double Dot(Matrix a, Matrix b)
        {
            if (!(a.Height() == 1 && b.Width() == 1 && a.Width() == b.Height()))
                throw new MatrixException("Dot product incompatible");

            double result = 0;

            for (int i = 0; i < a.Width(); i++)
            {
                result += a.get(0, i) * b.get(i, 0);
            }

            return result;

        }

        public static Matrix Transpose(Matrix a)
        {
            Matrix result = new Matrix(a.Width(), a.Height());

            for (int i = 0; i < a.Height(); i++)
            {
                for (int j = 0; j < a.Width(); j++)
                {
                    result.set(j, i, a.get(i, j));
                }
            }

            return result;
        }
        
        /*
         * Special matrix
         */
        public static Matrix Vandermonde(int n)
        {
            Matrix vandermonde = new Matrix(n + 1, n + 1);

            for (int i = 0; i <= n; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    vandermonde.set(i, j, Math.Pow(i, j));
                }
            }

            return vandermonde;
        }
        
        /*
         * Display
         */

        public static void Print(Matrix m)
        {

            for (int l = 0; l < m.Height(); l++)
            {
                Console.Write("[ " + centeredString(m.get(l, 0).ToString(), 6));
                for (int c = 1; c < m.Width(); c++)
                {
                    Console.Write(", " + centeredString(m.get(l, c).ToString(), 6));
                }
                Console.Write("]\n");
            }
            Console.Write("\n");
            
        }
        
        /*
         * Misc
         */
        
        static string centeredString(string s, int width)
        {
            if (s.Length >= width) s = s.Substring(0, width);

            int leftPadding = (width - s.Length) / 2;
            int rightPadding = width - s.Length - leftPadding;

            return new string(' ', leftPadding) + s + new string(' ', rightPadding);
        }
        
    }
}