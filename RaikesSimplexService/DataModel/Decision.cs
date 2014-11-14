using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace RaikesSimplexService.DataModel
{
    public class Decision
    {
        Object domain;
        string inspection;
        public Decision(Object domain, string inspection)
        //create two decision variables
        {
            this.domain = domain;
            this.inspection = inspection;
        }
    }
}
