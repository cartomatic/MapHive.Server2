using System;
using System.Collections.Generic;

namespace MapHive.Core.DataModel.Validation
{
    public class ValidationFailedException : Exception
    {
        public IList<IValidationError> ValidationErrors { get; set; } = new List<IValidationError>();
    } 
}
