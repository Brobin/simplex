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
        public double[,] rhs;
        public double[,] lhs;
        public double[] zRow;

        public Solver(Model model)
        {
            this.model = model;
            this.AddSlackSurplusVariables(model);
            this.createRhs();
            this.createLhs();
            this.createZRow();
        }

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
                var s = this.getSValue(c.Relationship, c.Value);
                var complete = false;
                for (int j = size; j < equality.Length; j++)
                {
                    if (!complete && j >= size + sUsed)
                    {
                        equality[j] = s;
                        complete = true;
                        sUsed++;
                    }
                }
                c.Coefficients = equality;
            }
            this.model = model;
            return model;
        }

        public double[] createZRow()
        {
            Goal g = this.model.Goal;
            double[] zRow = new double[g.Coefficients.Length + sVariables];
            for(int i = 0; i < g.Coefficients.Length; i++)
            {
                switch(this.model.GoalKind)
                {
                    case GoalKind.Maximize:
                        zRow[i] = 0 - g.Coefficients[i];
                        break;
                    case GoalKind.Minimize:
                        zRow[i] = g.Coefficients[i];
                        break;
                }
            }
            this.zRow = zRow;
            return zRow;
        }

        public double[,] createLhs()
        {
            var c = this.model.Constraints;
            int cons = c.Count;
            double[,] lhs = new double[1, cons];
            int size = c[0].Coefficients.Length;
            for(int i = 0; i < cons; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    var current = c[i].Coefficients[j];
                    var found = false;
                    if(current == 1 || current == -1)
                    {
                        found = true;
                        int k = 0;
                        while(found && k < cons)
                        {
                            var below = c[k].Coefficients[j];
                            if(below != 0)
                            {
                                if(k != i)
                                    found = false;
                            }
                            k++;
                        }
                    }
                    if (found)
                    {
                        lhs[0,i] = j;
                        break;
                    }
                }
            }
            this.lhs = lhs;
            return lhs;
        }

        public double[,] createRhs()
        {
            Model m = this.model;
            double[,] rhs = new double[1,m.Constraints.Count];
            for (int i = 0; i < m.Constraints.Count; i++ )
            {
                rhs[0,i] = m.Constraints[i].Value;
            }
            this.rhs = rhs;
            return rhs;
        }

        private int getSValue(Relationship r, double value)
        {
            var s = 0;
            switch (r)
            {
                case Relationship.GreaterThanOrEquals:
                    if (value != 0)
                        return -1;
                    break;
                case Relationship.LessThanOrEquals:
                    return 1;
            }
            return 0;
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
