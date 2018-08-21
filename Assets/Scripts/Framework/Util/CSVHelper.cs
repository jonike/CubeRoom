using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Sorumi.Util
{
    public class CSVHelper
    {
        public static string ArrayToString(string[,] array)
        {
            StringBuilder str = new StringBuilder("");
            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    str.Append(array[x, y] + "|");
                }
                str.Append("\n");
            }
            return str.ToString();
        }

        public static string[,] SplitCsv(string text)
        {
            string[] rows = text.Split('\n');
            int countOfRow = rows.Length;
            int countOfCol = SplitLine(rows[0]).Length;

            string[,] result = new string[countOfRow, countOfCol];
            for (int x = 0; x < countOfRow; x++)
            {
                string[] row = SplitLine(rows[x]);
                for (int y = 0; y < countOfCol; y++)
                {
                    result[x, y] = row[y];
                }
            }

            return result;
        }

        public static string[] SplitLine(string line)
        {
            string[] arrays = line.Split(';');
            return arrays;
        }
    }
}