using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaikesSimplexService.DataModel;

namespace RaikesSimplexService.DuckTheSystem
{
    public class DuckModel : Model
    {
        public override string ToString()
        {
            var output = "";
            switch (this.GoalKind)
            {
                case GoalKind.Minimize:
                    output += "Minimize\t";
                    break;
                case GoalKind.Maximize:
                    output += "Maximize\t";
                    break;
            }
            var goal = this.Goal;
            foreach(var c in goal.Coefficients)
            {
                output += c + "\t";
            }
            output += " = " + goal.ConstantTerm + "\n";
            foreach(var c in this.Constraints)
            {
                output += "\t\t";
                foreach(var x in c.Coefficients)
                {
                    output += x + "\t";
                }
                switch(c.Relationship)
                {
                    case Relationship.Equals:
                        output += "= ";
                        break;
                    case Relationship.GreaterThanOrEquals:
                        output += ">= ";
                        break;
                    case Relationship.LessThanOrEquals:
                        output += "<= ";
                        break;
                }
                output += c.Value + "\n";
            }
            return output;
        }
    }
}
