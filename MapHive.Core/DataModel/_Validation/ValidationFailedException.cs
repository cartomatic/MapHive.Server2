using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel.Validation
{
    public class ValidationFailedException : Exception
    {
        public IList<IValidationError> ValidationErrors { get; set; } = new List<IValidationError>();
    } 
}
