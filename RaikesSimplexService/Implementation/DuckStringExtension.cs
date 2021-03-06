﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaikesSimplexService.DataModel;

namespace RaikesSimplexService.DuckTheSystem
{
    /// <summary>
    /// DuckStringExtension contains our custom ToString methods so that we
    /// can override the defualt method without having to extend each model.
    /// </summary>
    public static class DuckStringExtension
    {
        /// <summary>
        /// Converts a Model to a string reprsentation
        /// </summary>
        /// <param name="model">the model</param>
        /// <param name="showSum">whether or not to show the sum</param>
        /// <returns></returns>
        public static string DuckString(this Model model, Boolean showSum)
        {
            var kind = model.GoalKind.DuckString();
            var goal = model.Goal.DuckString();
            var output = kind + "\t" + goal + "\n";
            foreach(var x in model.Constraints)
            {
                output += "\t\t" + x.DuckString(showSum) + "\n";
            }
            return output;
        }

        /// <summary>
        /// Converts a LinearConstraint to a string
        /// </summary>
        /// <param name="constraint">the constraint</param>
        /// <param name="showSum">Whether or not to show the sum</param>
        /// <returns></returns>
        public static string DuckString(this LinearConstraint constraint, Boolean showSum)
        {
            var output = String.Join("\t", constraint.Coefficients);
            if(showSum)
            {
                output += constraint.Relationship.DuckString();
                output += " " + constraint.Value;
            }
            return output;
        }

        /// <summary>
        /// Converts a Goal to a string
        /// </summary>
        /// <param name="goal">the Goal to convert</param>
        /// <returns></returns>
        public static string DuckString(this Goal goal)
        {
            var output = String.Join("\t", goal.Coefficients);
            output += " = " + goal.ConstantTerm;
            return output;
        }

        /// <summary>
        /// Converts a GoalKind Enumeration to a string
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static string DuckString(this GoalKind kind)
        {
            switch (kind)
            {
                case GoalKind.Minimize:
                    return "Minimize";
                default:
                    return "Maximize";
            }
        }

        /// <summary>
        /// Converts a Realtionship Enumeration to a string
        /// </summary>
        /// <param name="relationship"></param>
        /// <returns></returns>
        public static string DuckString(this Relationship relationship)
        {
            switch (relationship)
            {
                case Relationship.GreaterThanOrEquals:
                    return "\t>=\t";
                case Relationship.LessThanOrEquals:
                    return "\t<=\t";
                default:
                    return "\t=\t";
            }
        }

        public static string DuckString(this Solution solution)
        {
            string output = "";
            var counter = 1;
            foreach(double d in solution.Decisions)
            {
                output += "x" + counter + ": " + d + "\n";
                counter++;
            }
            output += "Optimal: " + solution.OptimalValue;
            return output;
        }

        public static string DuckString(this Solver solver)
        {
            var output = "Model: " + solver.modelMatrix.ToString();
            output  += "RHS" + solver.rhs.ToString();
            output += "\nLHS" + solver.lhs.ToString();
            output += "\nZ-Row" + solver.zRow.ToString();
            output += "\nB-Matrix" + solver.bMatrix.ToString();
            output += "\nZ-Vector" + solver.zVector.ToString();
            output += "\nXB' x RHS" + solver.xBPrime.ToString();
            output += "\nEntering: " + solver.Entering.ToString();
            output += "\nExiting: " + solver.exiting.ToString();
            return output;
        }
    }
}
