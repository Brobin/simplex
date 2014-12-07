using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Drawing.Drawing2D;
using RaikesSimplexService.Contracts;
using RaikesSimplexService.DataModel;
using MathNet.Numerics.LinearAlgebra;

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
        public Matrix<double> rhs;
        public Matrix<double> lhs;
        public Matrix<double> zRow;
        public Matrix<double> wRow;
        public Matrix<double> modelMatrix;
        public Matrix<double> bMatrix;
        public Matrix<double> bInverse;
        public Matrix<double> xBPrime;
        public Matrix<double> zVector;
        public List<Matrix<double>> pMatrices;
        public List<double> pPrimeList;
        public int Entering;
        public int exiting;
        public Solution solution;
        public bool twoPhase;



        /// <summary>
        /// Calls the methods that will create our matrices and prepare
        /// our model for running the simplex method.
        /// </summary>
        /// <param name="model"></param>
        private void SetUpModel(Model model)
        {
            this.model = this.removeUnnecessaryConstraints(model);
            this.AddSlackSurplusVariables();
            this.AddArtificialVariables();
            if (aVariables > 0)
            {
                this.twoPhase = true;
            }
            this.createZRow();
            this.createModelMatrix();
            this.createWRow();
            this.createRhs();
            this.createLhs();
        }

        /// <summary>
        /// Runs the simplex algorithm to find an optimal solution for a 
        /// given model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Solution Solve(Model model)
        {
            this.SetUpModel(model);
            if (twoPhase)
            {
    //FIRST SHIT
                //do shit
                //put wrow into old zrow spot
                this.zRow = this.wRow;
                //solve using wrow
                bool wloop = true;

                System.Diagnostics.Debug.WriteLine(this.modelMatrix.ToString());
                System.Diagnostics.Debug.WriteLine(this.rhs.ToString());
                System.Diagnostics.Debug.WriteLine(this.zRow.ToString());
                while (wloop)
                {
                    this.createBMatrixAndZVector();
                    this.bInverse = this.bMatrix.Inverse();
                    this.xBPrime = this.bInverse.Multiply(this.rhs);
                    this.getPMatrices();
                    this.zVectorXPMatrix();
                    wloop = this.getEnteringVariable();
                    if (wloop)
                    {
                        this.getExitingVariable();
                        this.updateLhs(this.Entering, this.exiting);
                    }
                }
                twoPhase = false;
                this.extractZRowAndColumn();
                this.getNewModelMatrix();
                this.createLhs();
            }
    //SECOND SHIT
            twoPhase = false;
            bool loop = true;
            while(loop)
            {
                this.createBMatrixAndZVector();
                this.bInverse = this.bMatrix.Inverse();
                this.xBPrime = this.bInverse.Multiply(this.rhs);
                this.getPMatrices();
                this.zVectorXPMatrix();
                loop = this.getEnteringVariable();
                double testEntering = this.Entering;
                if(loop)
                {
                    this.getExitingVariable();
                    this.updateLhs(this.Entering, this.exiting);
                }
            }
            this.sortLhs(this.lhs);
            this.solution = this.compileSolution();

            // 3. For each non basic variable
                    // Multiply the inverse of B with the each column of non basic variables (Pn')
                    // Multiple that result with the vector of the basic variables in the Z Row
                    // Subtract the result from the non basic variable in the z row
                // find the smallest of these, that is entering variable


            // 4. Find the entering column
                // (Xb' / Pn') * B'

            // repeat 2-5 until no negative values are in the z row
            return this.solution;
        }

        private Solution compileSolution()
        {
            int variables = this.modelMatrix.ColumnCount - this.sVariables;
            int constraints = this.modelMatrix.RowCount;
            double[] Decisions = new double[variables];
            for (int i = 0; i < constraints; i++)
            {
                int columnNum = (int)this.lhs[i, 0];
                if (columnNum < variables)
                {
                    Decisions[columnNum] = this.xBPrime[columnNum, 0];
                }
            }
            double OptimalValue = 0;
            double[] coeff = this.model.Goal.Coefficients;
            for(int j = 0; j < variables; j++)
            {
                OptimalValue += Decisions[j] * coeff[j];
            }

            Solution solution = new Solution(){
                Decisions = Decisions,
                OptimalValue = OptimalValue,
                AlternateSolutionsExist = false,
                Quality = SolutionQuality.Optimal
            };
            return solution;
        }


        private void extractZRowAndColumn()
        {
            int length = modelMatrix.ColumnCount - aVariables -1;
            int depth = model.Constraints.Count;
            double[,] newZRow = new double[1, length];
            for (int i = 1; i < length+1; i++)
            {
                newZRow[0, i-1] = this.modelMatrix[depth, i];
            }
            this.modelMatrix.RemoveColumn(0);
            this.zRow = Matrix<double>.Build.DenseOfArray(newZRow);
        }

        private void getNewModelMatrix()
        {
            var length = this.modelMatrix.ColumnCount - 1 - aVariables;
            var depth = this.modelMatrix.RowCount - 1;
            double[,] newMatrix = new double[depth, length];
            double[,] newRhs = new double[depth, 1];
            for (int i = 0; i < depth; i++ )
            {
                for(int j = 1; j < length+1; j++)
                {
                    newMatrix[i,j-1] = this.modelMatrix[i,j];
                }
                newRhs[i, 0] = this.rhs[i, 0];
            }
            this.modelMatrix = Matrix<double>.Build.DenseOfArray(newMatrix);
            this.rhs = Matrix<double>.Build.DenseOfArray(newRhs);
        }

        private void updateLhs(int entering, int exiting)
        {
            this.lhs[exiting, 0] = entering;
            List<double> newLhs = new List<double>();
            for(int i = 0; i < this.lhs.RowCount; i++)
            {
                newLhs.Add(this.lhs[i, 0]);
            }
            newLhs = this.sort(newLhs);
            for(int i = 0; i < this.lhs.RowCount; i++)
            {
                this.lhs[i, 0] = newLhs.ElementAt(i);
            }
        }

        private void getExitingVariable()
        {
            Matrix<double> pPrime = this.bInverse.Multiply(this.pMatrices[this.Entering]);
            double smallest = double.MaxValue;
            int exiting = -1;
            double[] ratios = new double[this.modelMatrix.RowCount];
            for(int i = 0; i < pPrime.RowCount; i++)
            {
                var current = this.xBPrime[i,0] / pPrime[i,0];
                if(current > 0 && current < smallest)
                {
                    smallest = current;
                    exiting = i;
                }
            }
            this.exiting = exiting;
        }

        private void getPMatrices()
        {
            this.pMatrices = new List<Matrix<double>>();
            List<double> lhsValues = new List<double>();
            int across = this.modelMatrix.ColumnCount;
            int down = this.modelMatrix.RowCount;
            for (int i = 0; i < this.lhs.RowCount; i ++)
            {
                lhsValues.Add(lhs[i,0]);
            }
            for (int i = 0; i < across; i++)
            {
                if (!lhsValues.Contains(i))
                {
                    double[,] p = new double[down,1];
                    for (int j = 0; j < down; j++)
                    {
                        p[j, 0] = this.modelMatrix[j, i];
                    }
                    this.pMatrices.Add(Matrix<double>.Build.DenseOfArray(p));
                }
                else
                {
                    double[,] p = new double[down, 1];
                    this.pMatrices.Add(Matrix<double>.Build.DenseOfArray(p));
                }
            }
        }

        private void sortLhs(Matrix<double> lhs)
        {
            List<double> array = new List<double>();
            for (int i = 0; i < lhs.RowCount; i++)
            {
                array.Add(lhs[i, 0]);
            }
            array = this.sort(array);
            for (int i = 0; i < lhs.RowCount; i++)
            {
                lhs[i, 0] = array.ElementAt(i);
            }
            this.lhs = lhs;
        }

        private List<double> sort(List<double> array)
        {
            var rand = new Random();
            if (array.Count > 1)
            {
                List<double> less = new List<double>();
                List<double> equal = new List<double>();
                List<double> greater = new List<double>();

                int pivot = rand.Next(0, array.Count - 1);
                double pivotValue = array[pivot];

                foreach (double a in array)
                {
                    if (a < pivotValue)
                        less.Add(a);
                    else if (a == pivotValue)
                        equal.Add(a);
                    else if (a > pivotValue)
                        greater.Add(a);
                }
                less = this.sort(less);
                greater = this.sort(greater);
                return less.Concat(equal).Concat(greater).ToList();
            }
            return array;
        }

        private void createBMatrixAndZVector()
        {
            int across = lhs.RowCount;
            int down = this.modelMatrix.RowCount;
            double[,] bMatrix2 = new double[down, across];
            double[,] z = new double[1, across];
            for (int i = 0; i < across; i++)
            {
                int lhsValue = (int)lhs[i,0];
                var value = this.zRow[0, lhsValue];
                z[0, i] = value;
                for (var k = 0; k < down; k++)
                {
                    bMatrix2[k, i] = this.modelMatrix[k, lhsValue];
                    
                }
            }
            this.bMatrix = Matrix<double>.Build.DenseOfArray(bMatrix2);
            this.zVector = Matrix<double>.Build.DenseOfArray(z);
        }

        private void zVectorXPMatrix()
        {
            List<double> pPrimeList = new List<double>();
            foreach(Matrix<double> p in this.pMatrices)
            {
                Matrix<double> pPrime = this.bInverse.Multiply(p);
                double pN = this.zVector.Multiply(pPrime)[0,0];
                pPrimeList.Add(pN);
            }
            this.pPrimeList = pPrimeList;
        }

        private bool getEnteringVariable()
        {
            double cPrime = double.MaxValue;
            int index = -1;
            List<double> lhsValues = new List<double>();
            for (int i = 0; i < this.lhs.RowCount; i++)
            {
                lhsValues.Add(lhs[i, 0]);
            }
            System.Diagnostics.Debug.WriteLine(this.lhs.ToString());
            for (int i = 0; i < this.modelMatrix.ColumnCount-aVariables; i++)
            {
                if(!lhsValues.Contains(i)){
                    var value = zRow[0, i] - (this.pPrimeList.ElementAt(i));
                    if (value < cPrime && value < 0)
                    {
                        cPrime = value;
                        index = i;
                    }
                }
            }
            if(cPrime == double.MaxValue)
            {
                return false;
            }
            this.Entering = index;
            return true;
            
        }

        

        /// <summary>
        /// Calculates how many slack and surplus variables there will be
        /// and adds them to each linear constraint
        /// </summary>
        public void AddSlackSurplusVariables()
        {
            this.sVariables = this.countSVariables();
            var sUsed = 0;
            foreach(var c in model.Constraints)
            {
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
                    if (!c.Relationship.Equals(Relationship.Equals))
                    {
                        if (!complete && j >= size + sUsed){
                            equality[j] = s;
                            complete = true;
                            sUsed++;
                        }
                    }
                }
                c.Coefficients = equality;
            }
        }

        public void AddArtificialVariables()
        {
            this.aVariables = this.countAVariables();
            var aUsed = 0;
            foreach (var c in model.Constraints)
            {
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
                    switch (c.Relationship)
                    {
                        case Relationship.GreaterThanOrEquals:
                            if (!complete && j >= size + aUsed)
                            {
                                equality[j] = a;
                                complete = true;
                                aUsed++;
                            }
                            break;
                    }
                }
                c.Coefficients = equality;
            }
        }

        /// <summary>
        /// Counts the number of S Variables needed for the model
        /// </summary>
        /// <returns></returns>
        private int countSVariables()
        {
            int size = 0;
            foreach (var c in this.model.Constraints)
            {
                if (this.getSValue(c.Relationship, c.Value) != 0)
                    size++;
            }
            return size;
        }

        /// <summary>
        /// Counts the number of artificial variables needed for the model
        /// </summary>
        /// <returns></returns>
        private int countAVariables(){
            int size = 0;
            foreach (var c in this.model.Constraints)
            {
                if (this.getAValue(c.Relationship, c.Value) != 0)
                    size++;
            }
            return size;
        }

        /// <summary>
        /// Creates the Z Row matrix for the simplex method
        /// </summary>
        public void createZRow()
        {
            Goal g = this.model.Goal;
            double[,] zRow = new double[1,g.Coefficients.Length + sVariables];
            for(int i = 0; i < g.Coefficients.Length; i++)
            {
                switch(this.model.GoalKind)
                {
                    case GoalKind.Maximize:
                        zRow[0,i] = 0 - g.Coefficients[i];
                        break;
                    case GoalKind.Minimize:
                        zRow[0,i] = g.Coefficients[i];
                        break;
                }
            }

            this.zRow = Matrix<double>.Build.DenseOfArray(zRow);


        }

        private void twoPhaseModelMatrix()
        {
            int lengthOfConstraint = this.model.Constraints[0].Coefficients.Length;
            int NumberofConstraints = this.model.Constraints.Count;
            double[,] modelMatrix2 = new double[NumberofConstraints+1, lengthOfConstraint+1];
            var zCount = 0;
            for (int i = 1; i < lengthOfConstraint+1; i++)
            {
                for (int j = 0; j < NumberofConstraints; j++)
                {
                    modelMatrix2[j, i] = this.model.Constraints[j].Coefficients[i-1];
                }
                if (zCount < lengthOfConstraint - aVariables)
                {
                    modelMatrix2[NumberofConstraints, i] = this.zRow[0, i-1];
                    zCount++;
                }
                else
                {
                    modelMatrix2[NumberofConstraints, i] = 0;
                }
            }
            for (int i = 0; i < NumberofConstraints; i++)
            {
                modelMatrix2[i, 0] = 0;
            }
            modelMatrix2[NumberofConstraints, 0] = 1;
            this.modelMatrix = Matrix<double>.Build.DenseOfArray(modelMatrix2);
        }

        private void singlePhaseModelMatrix()
        {
            int lengthOfConstraint = this.model.Constraints[0].Coefficients.Length;
            int NumberofConstraints = this.model.Constraints.Count;
            double[,] modelMatrix2 = new double[NumberofConstraints, lengthOfConstraint];
            for (int i = 0; i < lengthOfConstraint; i++)
            {
                for (int j = 0; j < NumberofConstraints; j++)
                {
                    modelMatrix2[j, i] = this.model.Constraints[j].Coefficients[i];
                }
            }
            this.modelMatrix = Matrix<double>.Build.DenseOfArray(modelMatrix2);
        }
        
        public void createModelMatrix()
        {
            if(aVariables!= 0){
                this.twoPhaseModelMatrix();
            } else {
                this.singlePhaseModelMatrix();
            }
        }
        /// <summary>
        /// Creates the W Row for the first phase of the two phase simplex method
        /// </summary>
        public void createWRow()
        {
            int numConstraints = this.model.Constraints.Count;
            //int numAVars = countAVariables();
            int numVars = modelMatrix.ColumnCount;
            double[,] wRow2 = new double[1,numVars];
            for (int i = 0; i < numVars - aVariables; i++)
            {
                double wCoeff = 0;
                for (int j = 0; j < numConstraints; j++)
                {
                    double aValue = 0;
                    for (int k = numVars - aVariables; k < numVars; k++)
                    {
                        aValue += this.modelMatrix[j, k];
                    }
                    if (aValue != 0)
                    {
                        wCoeff += this.modelMatrix[j, i];
                    }
                }
                wRow2[0,i] = -1*wCoeff;
            }
            this.wRow = Matrix<double>.Build.DenseOfArray(wRow2); ;
        }

        /// <summary>
        /// Creates the lhs, or Basic row for the simplex method
        /// </summary>
        /// <returns></returns>
        public void createLhs()
        {
            int cons = this.modelMatrix.RowCount;
            double[,] lhs = new double[cons,1];
            int size = modelMatrix.ColumnCount;
            for(int rows = 0; rows < cons; rows++)
            {
                for(int columns = 0; columns < size; columns++)
                {
                    var current = modelMatrix[rows, columns];
                    var found = false;
                    if(current == 1 || (!twoPhase && current == -1))
                    {
                        found = true;
                        int k = 0;
                        while(found && k < cons)
                        {
                            var below = modelMatrix[k,columns];
                            if (below != 0)
                            {
                                if (k != rows)
                                {
                                    found = false;
                                }
                            }
                            k++;
                        }
                    }
                    if (found) {
                        lhs[rows, 0] = columns;
                        break;
                    }
                }
            }
            var x = Matrix<double>.Build.DenseOfArray(lhs);
            this.sortLhs(x);
        }

        /// <summary>
        /// Creates the Rhs matrix for the simplex method
        /// </summary>
        public void createRhs()
        {
            int length = this.model.Constraints.Count;
            if (twoPhase)
            {
                length += 1;
            }
            double[,] rhs = new double[length,1];
            if (twoPhase)
            {
                for (int i = 0; i < length - 1; i++)
                {
                    rhs[i, 0] = this.model.Constraints[i].Value;
                }
                rhs[length - 1, 0] = 0;
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    rhs[i, 0] = this.model.Constraints[i].Value;
                }
            }
            this.rhs = Matrix<double>.Build.DenseOfArray(rhs); 
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
            switch (r) {
                case Relationship.GreaterThanOrEquals:
                    if (value != 0)
                        s = -1;
                    break;
                case Relationship.LessThanOrEquals:
                    s = 1;
                    break;
            }
            return s;
        }

        /// <summary>
        /// Overwrites model and gets rid of not-negative constraints
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
                        j--;
                    }
                }
            }
            this.model = matrixModel;
            return matrixModel;
            
        }

        /// <summary>
        /// Returnt he value of an artificial variable (0, 1) based on the relationship
        /// and the rhs value
        /// </summary>
        /// <param name="r"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private int getAValue(Relationship r, double value)
        {
            if (r.Equals(Relationship.GreaterThanOrEquals) && value != 0)
            {
                return 1;
            }   
            return 0;
        }
    }
}
