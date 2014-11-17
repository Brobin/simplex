using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaikesSimplexService.DataModel;

namespace RaikesSimplexService.DuckTheSystem
{
    public static class DuckStringExtension
    {
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

        public static string DuckString(this LinearConstraint constraint, Boolean showSum)
        {
            var output = "";
            foreach (var x in constraint.Coefficients)
            {
                output += x + "\t";
            }
            if(showSum)
            {
                output += constraint.Relationship.DuckString();
                output += " " + constraint.Value;
            }
            return output;
        }

        public static string DuckString(this Goal goal)
        {
            var output = "";
            foreach (var c in goal.Coefficients)
            {
                output += c + "\t";
            }
            output += " = " + goal.ConstantTerm;
            return output;
        }

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

        public static string DuckString(this double[,] matrix, string type)
        {
            var output = type + ":\n";
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                output += "\t" + matrix[0, i] + "\n";
            }
            return output;
        }

        public static string DuckString(this double[] array, string Title)
        {
            var output = Title + ":\n";
            foreach (double d in array)
            {
                output += "\t" + d;
            }
            return output;
        }
    }
}
