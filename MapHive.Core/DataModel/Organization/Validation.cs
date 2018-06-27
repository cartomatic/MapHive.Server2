using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MapHive.Core.DataModel.Validation;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif


namespace MapHive.Core.DataModel
{
    public partial class Organization
    {
        public override IEnumerable<IValidator> GetValidators()
        {
            var validators = new List<IValidator>();
            var baseValidators = base.GetValidators();
            if (baseValidators != null)
            {
                validators.AddRange(baseValidators);
            }
            validators.Add(new OrganizationValidator());
            return validators;
        }

        /// <summary>
        /// Configuration for validation
        /// </summary>
        public class OrganizationValidator : AbstractValidator<Organization>
        {
            public OrganizationValidator()
            {
                RuleFor(x => x.Slug).WithValueRequired();
            }
        }

        /// <summary>
        /// validates the org slug for uniqueness
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        protected override async Task ValidateAgainstDbAsync(DbContext dbCtx)
        {
            var slugTaken = !string.IsNullOrEmpty(Slug) &&
                await dbCtx.Set<Organization>().AnyAsync(o => o.Uuid != Uuid && o.Slug == Slug)
                || await dbCtx.Set<MapHiveUser>().AnyAsync(u => u.UserOrgId != Uuid && u.Slug == Slug);


            if (slugTaken)
            {
                var validationFailedException = new ValidationFailedException();
                validationFailedException.ValidationErrors.Add(new ValidationError
                {
                    Message = "Organization slug already taken.",
                    Code = "slug_taken",
                    PropertyName = nameof(Slug)
                });

                throw validationFailedException;
            }
        }
    }
}
