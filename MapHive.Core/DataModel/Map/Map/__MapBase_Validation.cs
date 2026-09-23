using System.Collections.Generic;
using FluentValidation;
using MapHive.Core.DataModel.Validation;

namespace MapHive.Core.DataModel.Map
{
    public partial class MapBase
    {
        public override IEnumerable<IValidator> GetValidators()
        {
            var validators = new List<IValidator>();
            var baseValidators = base.GetValidators();
            if (baseValidators != null)
            {
                validators.AddRange(baseValidators);
            }
            validators.Add(new MapValidator());
            return validators;
        }

        /// <summary>
        /// Project validator
        /// </summary>
        public class MapValidator : AbstractValidator<MapBase>
        {
            public MapValidator()
            {
                RuleFor(x => x.Name).WithValueRequired().WithLength(1, 254);
            }
        }
    }
}
