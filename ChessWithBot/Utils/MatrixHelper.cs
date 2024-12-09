using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWithBot.Utils
{
    public static class MatrixHelper
    {
        public static double[,] RotateRows(this double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] reversed = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    reversed[i, j] = matrix[rows - 1 - i, j];
                }
            }

            return reversed;
        }
    }
}
