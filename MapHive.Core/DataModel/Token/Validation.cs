using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace MapHive.Core.DataModel
{
    public partial class Token
    {
        public override IEnumerable<IValidator> GetValidators()
        {
            var validators = new List<IValidator>();
            var baseValidators = base.GetValidators();
            if (baseValidators != null)
            {
                validators.AddRange(baseValidators);
            }
            validators.Add(new TokenValidator());
            return validators;
        }

        /// <summary>
        /// Configuration for validation
        /// </summary>
        public class TokenValidator : AbstractValidator<Token>
        {
            public TokenValidator()
            {
                RuleFor(x => x.OrganizationId).NotNull();
                RuleFor(x => x.ApplicationIds).NotNull();
                RuleFor(x => x.ApplicationIds.Count).GreaterThan(0);
            }
        }
    }
}
