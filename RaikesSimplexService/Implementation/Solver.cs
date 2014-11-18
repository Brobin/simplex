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
    
    /// <summary>
    /// This class takes a Linear Programming Model and using the revised
    /// simplex method will be able to find an optimal solution to problems
    /// of any size.
    /// </summary>
    public class Solver : ISolver
    {
        public Model model { get; set; }
        private int sVariables;
        private int aVariables;
        public double[,] rhs;
        public double[,] lhs;
        public double[] zRow;

        /// <summary>
        /// Constuctor, calls the method to set up the model for running the
        /// simplex method on the given model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Solution Solve(Model model)
        {
            this.SetUpModel(model);
            //throw new NotImplementedException();
            return null;
        }

        /// <summary>
        /// Calls the methods that will create our matrices and prepare
        /// out model for running the simplex method.
        /// </summary>
        /// <param name="model"></param>
        private void SetUpModel(Model model)
        {
            this.model = model;
            //this.removeUnnecessaryConstraints(model);
            this.AddSlackSurplusVariables(model);
            this.AddArtificialVariables(model);
            this.createRhs();
            this.createLhs();
            this.createZRow();
        }

        public Model removeUnnecessaryConstraints(Model model)
        {
            Model matrixModel = model;
            for (int j = 0; j < matrixModel.Constraints.Count; j++)
            {
                var c = matrixModel.Constraints.ElementAt(j);
                if (c.Value == 0)
                {
                    int numVariables = 0;
                    for (int i = 0; i < c.Coefficients.Length; i++)
                    {
                        if (c.Coefficients[i] != 0)
                        {
                            numVariables++;
                        }
                    }
                    if (numVariables == 1)
                    {
                        matrixModel.Constraints.RemoveAt(j);
                    }
                }
            }

            return matrixModel;
        }

        /// <summary>
        /// Calculates how many slack and surplus variables there will be
        /// and adds them to each linear constraint
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Model AddSlackSurplusVariables(Model model)
        {
            
            this.model = model;//Do we need this at beginning and end of method?
            //this.sVariables = model.Constraints.Count;
            this.sVariables = this.countSVariables(model);
            var sUsed = 0;
            foreach(var c in model.Constraints)
            {
                Relationship y = c.Relationship;
                //get to specific coefficient = model.Constraints.ElementAt(0).Coefficients[0]

               var size = c.Coefficients.Length;

             
                double[] equality = new double[size + this.sVariables];
                for(int i = 0; i < size; i++)
                {
                    equality[i] = c.Coefficients[i];
                }
                Relationship f = c.Relationship;
                var s = this.getSValue(c.Relationship, c.Value);
                var complete = false;
                for (int j = size; j < equality.Length; j++)
                {
                    switch (c.Relationship) { 
                        case Relationship.GreaterThanOrEquals:
                            if (!complete && j >= size + sUsed)
                            {
                                equality[j] = s;
                                complete = true;
                                sUsed++;
                            }
                            break;
                        case Relationship.LessThanOrEquals:
                            if (!complete && j >= size + sUsed)
                            {
                                equality[j] = s;
                                complete = true;
                                sUsed++;
                            }
                            break;
                    }
                }
                Relationship d = c.Relationship;
                c.Coefficients = equality;
                Relationship r = c.Relationship;
            }
            this.model = model;
            return model;
        }

        public Model AddArtificialVariables(Model model)
        {
            this.model = model;
            this.aVariables = this.countAVariables(model);
            var aUsed = 0;
            foreach (var c in model.Constraints)
            {
                //get to specific coefficient = model.Constraints.ElementAt(0).Coefficients[0]

                var size = c.Coefficients.Length;

                double[] equality = new double[size + this.aVariables];
                for (int i = 0; i < size; i++)
                {
                    equality[i] = c.Coefficients[i];
                }
                int a = this.getAValue(c.Relationship, c.Value);
             
                var complete = false;
                for (int j = size; j < equality.Length; j++)
                {
                    switch (c.Relationship) {
                        case Relationship.GreaterThanOrEquals:
                            if (!complete && j >= size + aUsed)
                            {
                                equality[j] = a;//
                                complete = true;
                                aUsed++;
                            }
                            break;
                    }
                }
                c.Coefficients = equality;
            }
            this.model = model;


            return model;

        }

        private int countSVariables(Model model)
        {
            int size = 0;
            foreach (var c in model.Constraints)
            {
                int returnValue = this.getSValue(c.Relationship, c.Value);
                if (returnValue != 0)
                {
                    size++;
                }
            }

            return size;
        }

        private int countAVariables(Model model)
        {
            int size = 0;
            foreach (var c in model.Constraints)
            {
                int returnValue = this.getAValue(c.Relationship, c.Value);
                if (returnValue != 0)
                {
                    size++;
                }
            }

            return size;
        }

       

        /// <summary>
        /// Creates the Z Row matrix for the simplex method
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Creates the lhs, or Basic row for the simplex method
        /// </summary>
        /// <returns></returns>
        public double[,] createLhs()
        {
            var c = this.model.Constraints;
            int cons = c.Count;
            double[,] lhs = new double[1, cons];
            int size = c[0].Coefficients.Length;
            for(int rows = 0; rows < cons; rows++)
            {
                for(int columns = 0; columns < size; columns++)
                {
                    var current = c[rows].Coefficients[columns];
                    var found = false;
                    if(current == 1 || current == -1)
                    {
                        found = true;
                        int k = 0;
                        while(found && k < cons)
                        {
                            var below = c[k].Coefficients[columns];
                            if(below != 0)
                            {
                                if(k != rows)
                                    found = false;
                            }
                            k++;
                        }
                    }
                    if (found)
                    {
                        lhs[0,rows] = columns;
                        break;
                    }
                }
            }
            this.lhs = lhs;
            return lhs;
        }

        /// <summary>
        /// Creates the Rhs matrix for the simplex method
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the value of a slack variable (0, 1, -1) based on the relationship
        /// and the rhs value
        /// </summary>
        /// <param name="r"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
            return s;
        }

        private int getAValue(Relationship r, double value)
        {
            int a = 0;
            Relationship r2 = r;
            if (r.Equals(Relationship.GreaterThanOrEquals) && value != 0){

                a = 1;
      
            }   
            return a;
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
