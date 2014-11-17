using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Drawing.Drawing2D;
using RaikesSimplexService.Contracts;
using RaikesSimplexService.DataModel;

namespace RaikesSimplexService.DuckTheSystem
{
    
    public class Solver : ISolver
    {
        public Model model { get; set; }
        private int sVariables;

        public Solution Solve(Model model)
        {
            throw new NotImplementedException();
        }

        public Model AddSlackSurplusVariables(Model model)
        {
            this.model = model;
            this.sVariables = model.Constraints.Count;
            var sUsed = 0;
            foreach(var c in model.Constraints)
            {
                //get to specific coefficient = model.Constraints.ElementAt(0).Coefficients[0]
                var size = c.Coefficients.Length;
                double[] equality = new double[size + this.sVariables];
                for(int i = 0; i < size; i++)
                {
                    equality[i] = c.Coefficients[i];
                }
                var s = 0;
                switch (c.Relationship)
                {
                    case Relationship.GreaterThanOrEquals:
                        if (c.Value != 0)
                            s = -1;
                        break;
                    case Relationship.LessThanOrEquals:
                        s = 1;
                        break;
                }
                var complete = false;
                for (int j = size; j < equality.Length; j++)
                {
                    if(!complete && j >= size + sUsed)
                    {
                        equality[j] = s;
                        complete = true;
                        sUsed++;
                    }
                    else
                    {
                        equality[j] = 0;
                    }
                }

                c.Coefficients = equality;
            }
            return model;
        }

        public Matrix[,] fillInMatrix(Model model)
        {
            int rows = model.Constraints.Count;
            int columns = model.Constraints.ElementAt(0).Coefficients.Length;
            Matrix[,] matrix = new Matrix[rows, columns];
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < columns; j++) {
                   // matrix[i,j] = model.Constraints.ElementAt(i).Coefficients[j];
                }
            }
            return matrix;
        }

    }
}
