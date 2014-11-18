using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using RaikesSimplexService.DataModel;


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

       /* public void AddConstraints(string name, params string[] inequalities)
        {
            for (int i = 0; i < inequalities.Length; i++) {
                string[] parts1 = inequalities[i].Split('=');
                String[] parts2 = Regex.Split(parts1[0], @"(?=[+-])");
                double[] coefficients = new double[parts2.Length];
                int x = 0;
                foreach (String part in parts2)
                {
                    String coeff = "";
                    if (part.Substring(0, 1).Equals("-"))
                    {
                        coeff = "-";
                    }
                    coeff += Regex.Replace(part, "[^0-9.]", "");
                    System.Diagnostics.Debug.WriteLine(coeff);
                    coefficients[x] = Convert.ToDouble(coeff);
                    x++;
                }
                double term = Convert.ToDouble(parts1[1]);
                Relationship relationship;
                if (parts2[parts2.Length-1].Contains("<")) {
                    relationship = Relationship.LessThanOrEquals;
                } else if (parts2[parts2.Length-1].Contains(">")) { 
                    relationship = Relationship.GreaterThanOrEquals;
                } else { 
                    relationship = Relationship.Equals;
                }
                LinearConstraint linConstr = new LinearConstraint() { Coefficients = coefficients, Relationship = relationship, Value = Convert.ToDouble(parts1[1]) };
            } //put all linear constraints found in for loop into a list of constraints
        }

        public void AddGoal(string description, GoalKind goalKind, String equation)
        {
            String[] parts1 = equation.Split('=');
            String[] parts2 = Regex.Split(parts1[0], @"(?=[+-])");
            List<String> nonEmpty = new List<String>();
            foreach (string p in parts2)
            {
                if (p != "")
                {
                    nonEmpty.Add(p);
                }
            }
            double[] coefficients = new double[nonEmpty.Count];
            int x = 0;
            foreach (String part in nonEmpty)
            {
                String coeff = "";
                coeff += Regex.Replace(part, "[^0-9.-]", "");
                coefficients[x] = Convert.ToDouble(coeff);
                x++;
            }
            this.GoalKind = goalKind;
            double term = Convert.ToDouble(parts1[1]);
            Goal finalGoal = new Goal() { Description = description, Coefficients = coefficients, ConstantTerm = term };
            this.Goal = finalGoal;

        } */

        public override string ToString()
        {
            String myString = "Model:\t\tX\tY\r\n";
            myString += this.GoalKind + "\t" + this.Goal.ToString();
            return myString;
        }
    }
}
