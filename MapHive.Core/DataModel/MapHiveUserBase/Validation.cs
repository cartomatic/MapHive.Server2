﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MapHive.Core.DataModel.Validation;

namespace MapHive.Core.DataModel
{
    public abstract partial class MapHiveUserBase
    {
        public override IEnumerable<IValidator> GetValidators()
        {
            var validators = new List<IValidator>();
            var baseValidators = base.GetValidators();
            if (baseValidators != null)
            {
                validators.AddRange(baseValidators);
            }
            validators.Add(new UserValidator());
            return validators;
        }

        /// <summary>
        /// Configuration for validation
        /// </summary>
        public class UserValidator : AbstractValidator<MapHiveUserBase>
        {
            public UserValidator()
            {
                RuleFor(x => x.Email).WithValueRequired().WithValidEmailAddress();
            }
        }
    }
}
