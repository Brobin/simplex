using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaikesSimplexService.DataModel;

namespace RaikesSimplexService.DuckTheSystem
{
    public static class DuckString
    {
        public static string DuckString(this Model model)
        {
            var kind = model.GoalKind.DuckString();
            var goal = model.Goal.DuckString();
            var output = kind + "\t" + goal + "\n";
            foreach(var x in model.Constraints)
            {
                output += "\t\t" + x.DuckString() + "\n";
            }
            return output;
        }

        public static string DuckString(this LinearConstraint constraint)
        {
            var output = "";
            foreach (var x in constraint.Coefficients)
            {
                output += x + "\t";
            }
            output += constraint.Relationship.DuckString();
            output += " " + constraint.Value;
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
    }
}
