using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaikesSimplexService.DataModel;

namespace RaikesSimplexService.DuckTheSystem
{
    public static class ExtensionClass
    {
        public static string DuckString(this Model model)
        {
            var goal = model.Goal.DuckString();
            var kind = model.GoalKind.DuckString();
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
                    break;
                default:
                    return "Maximize";
                    break;
            }
        }

        public static string DuckString(this Relationship relationship)
        {
            switch (relationship)
            {
                case Relationship.GreaterThanOrEquals:
                    return ">= ";
                    break;
                case Relationship.LessThanOrEquals:
                    return "<=";
                    break;
                default:
                    return "=";
                    break;
            }
        }
    }
}
