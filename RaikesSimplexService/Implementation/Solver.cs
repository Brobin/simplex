using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using RaikesSimplexService.Contracts;
using RaikesSimplexService.DataModel;

namespace RaikesSimplexService.DuckTheSystem
{
    
    public class Solver : ISolver
    {
        public Solution Solve(Model model)
        {
           var newModel = this.AddSlackSurplusVariables(model);
           throw new NotImplementedException();
        }

        public Model AddSlackSurplusVariables(Model model)
        {
            var slack = 0;
            var artificial = 0;
            foreach(var c in model.Constraints)
            {
                switch(c.Relationship)
                {
                    case Relationship.GreaterThanOrEquals:
                        if(c.Value != 0)
                        {
                            slack += 1;
                            artificial += 1;
                        }
                        break;
                    case Relationship.LessThanOrEquals:
                        slack += 1;
                        break;
                }
            }
            var usedSlack = 0;
            var usedArtificial = 0;
            foreach(var c in model.Constraints)
            {
                var size = c.Coefficients.Length;
            }

            return null;
        }
    }
}
