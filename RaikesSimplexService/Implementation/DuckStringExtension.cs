using System;
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
                    return ">= ";
                case Relationship.LessThanOrEquals:
                    return "<=";
                default:
                    return "=";
            }
        }

        /// <summary>
        /// Converts a 2D double array (or matrix) to a string with a given title
        /// </summary>
        /// <param name="matrix">the 2D array</param>
        /// <param name="type">title of the matrix</param>
        /// <returns></returns>
        public static string DuckString(this double[,] matrix, string type)
        {
            var output = type + ":\n";
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                output += "\t" + matrix[0, i] + "\n";
            }
            return output;
        }

        /// <summary>
        /// Converts a 1D double array to a string
        /// </summary>
        /// <param name="array">the 1D array</param>
        /// <param name="Title">the title</param>
        /// <returns></returns>
        public static string DuckString(this double[] array, string Title)
        {
            var output = Title + ":\n";
            output += String.Join("\t", array) + "\n";
            return output;
        }
    }
}
