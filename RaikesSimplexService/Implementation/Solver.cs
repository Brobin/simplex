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
        public Solution Solve(Model model)
        {
           var newModel = this.AddSlackSurplusVariables(model);
           throw new NotImplementedException();
        }

        public Model AddSlackSurplusVariables(Model model)
        {
            var slack = 0;
            var artificial = 0;
            foreach(var c in model.Constraints)
            {
                switch(c.Relationship)
                {
                    case Relationship.GreaterThanOrEquals:
                        if(c.Value != 0)
                        {
                            slack += 1;
                            artificial += 1;
                        }
                        break;
                    case Relationship.LessThanOrEquals:
                        slack += 1;
                        break;
                }
            }
            var usedSlack = 0;
            var usedArtificial = 0;
            foreach(var c in model.Constraints)
            {
                //get to specific coefficient = model.Constraints.ElementAt(0).Coefficients[0]
                var size = c.Coefficients.Length;
                double[] equality = new double[size + slack + artificial];
                switch (c.Relationship)
                {
                    case Relationship.GreaterThanOrEquals:
                        if (c.Value != 0)
                        {
                            //at size plus usedSlack make coeff -1
                            //at size plus slack plus usedArtificial make coeff 1
                            //fill an array of new size with correct coeffs then set this array as coeff array in constraint
                            
                            for (int i = 0; i < size; i++)
                            {
                                equality[i] = c.Coefficients[i];
                            }
                            for (int j = size; j < size + slack; j++)
                            {
                                if (j != size + usedSlack)
                                {
                                    equality[j] = 0;
                                }
                                else
                                {
                                    equality[j] = -1;
                                    usedSlack++;
                                }
                            }
                            for (int j = size+slack; j < equality.Length; j++)
                            {
                                if (j != size+slack + usedArtificial)
                                {
                                    equality[j] = 0;
                                }
                                else
                                {
                                    equality[j] = 1;
                                    usedArtificial++;
                                }
                            }
                        }
                        break;
                    case Relationship.LessThanOrEquals:
                        //at size plus usedSlack make coeff 1
                        for (int j = size; j < equality.Length; j++)
                        {
                            if (j != size + usedSlack)
                            {
                                equality[j] = 0;
                            }
                            else
                            {
                                equality[j] = 1;
                                usedSlack++;
                            }
                        }
                        break;
                }
                c.Coefficients = equality;
            }
            return model;
        }

        public Matrix fillInMatrix(Model model)
        {
            int rows = model.Constraints.Count;
            int columns = model.Constraints.ElementAt(0).Coefficients.Length;
            Matrix[,] matrix = new Matrix[rows, columns];
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < columns; j++) {
                    matrix[i,j] = (float)model.Constraints.ElementAt(i).Coefficients[j];
                }
            }
            return matrix;
        }

    }
}
