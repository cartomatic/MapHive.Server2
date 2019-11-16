using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MapHive.Core.DataModel.Validation;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel.Map
{
    public partial class Layer
    {
        public override IEnumerable<IValidator> GetValidators()
        {
            var validators = new List<IValidator>();
            var baseValidators = base.GetValidators();
            if (baseValidators != null)
            {
                validators.AddRange(baseValidators);
            }
            validators.Add(new LayerValidator());
            return validators;
        }

        /// <summary>
        /// Client validator
        /// </summary>
        public class LayerValidator : AbstractValidator<Layer>
        {
            public LayerValidator()
            {
                RuleFor(x => x.Name).WithValueRequired().WithLength(1, 254);
            }
        }
    }
}
