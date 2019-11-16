using FluentValidation;
using System.Collections.Generic;

namespace MapHive.Core.DataModel.Map
{
    public partial class DataStore
    {
        public override IEnumerable<IValidator> GetValidators()
        {
            var validators = new List<IValidator>();
            var baseValidators = base.GetValidators();
            if (baseValidators != null)
            {
                validators.AddRange(baseValidators);
            }
            validators.Add(new ShapeFileValidator());
            return validators;
        }

        /// <summary>
        /// DataStore validator
        /// </summary>
        public class ShapeFileValidator : AbstractValidator<DataStore>
        {
            public ShapeFileValidator()
            {

            }
        }
    }
}
