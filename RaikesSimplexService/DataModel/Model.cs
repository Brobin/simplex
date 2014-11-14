using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;


namespace RaikesSimplexService.DataModel
{
    /// <summary>
    /// Defines a model that has expressions and constraints.
    /// </summary>
    [DataContract]
    public class Model
    {
        /// <summary>
        /// Data member that contains the function you want to optimize.
        /// </summary>
        [DataMember]
        public Goal Goal { get; set; }

        /// <summary>
        /// Data member that contains a list of constraint equations.
        /// </summary>
        [DataMember]
        public List<LinearConstraint> Constraints { get; set; }

        /// <summary>
        /// Data member that indicated whether to maximize of minimize the goal function.
        /// </summary>
        [DataMember]
        public GoalKind GoalKind { get; set; }

        public void AddDecisions(params Decision[] variables)
        {

        }

        public void AddConstraints(string name, params string[] inequalities)
        {

        }

        public void AddGoal(string description, GoalKind goalKind, String equation)
        {
            String[] parts1 = equation.Split('=');
            String[] parts2 = Regex.Split(parts1[0], @"(?=[+-])");
            double[] coefficients = new double[parts2[0].Length];
            int x = 0;
            foreach (String part in parts2)
            {
                String coeff = "";
                coeff += Regex.Replace(part, "[^0-9.-]", "");
                System.Diagnostics.Debug.WriteLine(coeff);
                coefficients[x] = Convert.ToDouble(coeff);
                x++;
            }
            this.GoalKind = goalKind;
            double term = Convert.ToDouble(parts1[1]);
            Goal finalGoal = new Goal() { Description = description, Coefficients = coefficients, ConstantTerm = term };
            this.Goal = finalGoal;

        }

        public override string ToString()
        {
            string myString = "";
            foreach (int x in this.Goal.Coefficients)
            {
                myString += x.ToString() + " ";
                System.Diagnostics.Debug.WriteLine(myString);
            }
            return myString;
        }
    }
}
