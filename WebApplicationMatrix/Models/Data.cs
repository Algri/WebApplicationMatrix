using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WebApplicationMatrix.Models
{
    public class Data
    {
        [Display(Name = "Input:")]
        public string Input { get; set; }

        [Display(Name = "Error input:")]
        public List<string> Error { get; set; }

        List<string> RowParserStr;
        int[,] arrayData;

        public Data()
        {
            RowParserStr = new List<string>();
            Error = new List<string>();
            arrayData = null;
        }

        public int GetOutputResult()
        {
            int count = 0;
            try
            {                
                bool isSucess = IsDataChecking();
                if (isSucess)
                {
                    // Get the number of columns
                    int length = arrayData.GetLength(0);
                    // Get odd hand integer N (3 ≤ N ≤ 2000)
                    int[] odd = GetOddNumber(length);                    
                    count = GetOutputCount(odd);
                }                
            }
            catch (Exception)
            {
                Error.Add("Input is not correct");                
            }
            
            return count;
        }

        private bool IsDataChecking()
        {
            bool isParse = ParserInputData();
            if (!isParse) return false;
            bool isConvertSuccess = ConvertToMatrix();
            if(!isConvertSuccess) return false;
            bool isValidValues = ValidationValuesMatrix();
            if (!isValidValues) return false;
            return true;
        }


        private int GetOutputCount(int[] odd)
        {
            int count = 0;
            foreach (var item in odd)
            {
                int[,] searchPattern = new int[item, item];
                FillTemplateMatrix(searchPattern);
                count += SearchCompliance(arrayData, searchPattern);                
            }
            return count;
        }

        public bool ParserInputData()
        {
            if (String.IsNullOrEmpty(Input)) { Error.Add("Input field is empty"); return false; }

            string[] value = Input.Replace(" ", "").Split("\r\n");

            foreach (var item in value)
            {
                RowParserStr.Add(item);
            }
            return true;
        }

        public bool ConvertToMatrix()
        {
            // Check row == col
            bool isValidDimension = IsValidateSquareMatrix();            

            if (isValidDimension)
            {                
                int length = RowParserStr.Count();
                arrayData = new int[length, length];                
                int y = 0;

                foreach (var item in RowParserStr)
                {
                    int x = 0;
                    foreach (var value in item)
                    {
                        bool success = Int32.TryParse(value.ToString(), out int number);
                        if (success)
                        {
                            arrayData[y, x] = number;
                            x++;
                        }                        

                    }                    
                    y++;                    
                }
                return true;
            }
            return false;
        }

        private bool IsValidateSquareMatrix()
        {
            Error.Clear();
            int numberRowsMatrix = RowParserStr.Count();
            string firstLine = RowParserStr.First();
            bool success = Int32.TryParse(firstLine.ToString(), out int number);
            bool isCorrect = true;
            if (success)
            {
                RowParserStr.RemoveAt(0);
                isCorrect = (number < 3 || number > 2000) ? false : true;
                if (!isCorrect) Error.Add("The first line should contain an integer N (3 ≤ N ≤ 2000)");
                if (number != RowParserStr.Count()) { Error.Add("The dimension of the matrix is not correct"); return false; }
                isCorrect = RowParserStr.All(x => x.Length == RowParserStr.Count());
                if (!isCorrect) { Error.Add("The matrix entered is not square"); return false; }
                
                if (number != RowParserStr.Count()) { Error.Add("Input matrix is empty"); return false; };                
            }           

            return isCorrect; 
        }

        private bool IsEqualMatrix(int[,] data1, int[,] data2)
        {
            var equal = Enumerable.Range(0, data1.Rank).All(d =>
                data1.Cast<int>().SequenceEqual(data2.Cast<int>()));
            return equal;
        }

        private int[] GetOddNumber(int number)
        {
            return Enumerable.Range(3, number).Where(i => i % 2 == 1 && i <= number).ToArray();
        }

        private int[,] FillTemplateMatrix(int[,] matrix)
        {
            // Get the number of columns
            int center = matrix.GetLength(0) / 2;
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    if (x == center || y == center)
                    {
                        matrix[y, x] = 1;
                    }
                }
            }
            return matrix;
        }

        // Match Search
        private int SearchCompliance(int[,] data, int[,] searchPattern)
        {
            int width = searchPattern.GetLength(0);
            int height = width;
            int[,] fragmentSearchMatrix = new int[width, height];
            int dX = 0;
            int dY = 0;
            int n = 0;
            for (int i = 0; i <= data.GetLength(1) - height; i++) // Y
            {
                for (int j = 0; j <= data.GetLength(0) - width; j++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            fragmentSearchMatrix[y, x] = data[y + dY, x + dX];
                        }
                    }
                    
                    bool eq = IsEqualMatrix(fragmentSearchMatrix, searchPattern);
                    if (eq)
                        n++;
                    dX++;
                }
                dY++; dX = 0;
            }
            return n;
        }

        // Check that all values are 0 and 1
        private bool ValidationValuesMatrix()
        {
            bool isValidValues = arrayData.Cast<int>().All(x => x == 0 || x == 1);
            if(!isValidValues) Error.Add("The matrix must contain only 0 and 1");
            return isValidValues;
        }
    }
}
