using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Validators;

namespace MapHive.Core.DataModel.Validation
{
    /// <summary>
    /// Validation errors
    /// </summary>
    public static class ValidationErrors
    {
        public static IValidationError ValueRequired { get; } = new ValidationError()
        {
            Code = "value_required",
            Message = "Value for the field is required."
        };

        public static IValidationError InvalidLength { get; } = new ValidationError()
        {
            Code = "invalid_length",
            Message = "Invalid field length."
        };

        public static IValidationError InvalidEmail { get; } = new ValidationError()
        {
            Code = "invalid_email",
            Message = "Invalid email."
        };

        public static IValidationError EmailInUse { get; } = new ValidationError
        {
            Code = "email_in_use",
            Message = "Email in use."
        };

        public static IValidationError UniqueConstraint { get; } = new ValidationError
        {
            Code = "unique_constraint",
            Message = "Field value must be unique."
        };

        public static IValidationError NoIdentifier { get; } = new ValidationError
        {
            Code = "no_identifier",
            Message = "Dataset requires a unique identifier field."
        };

        public static IValidationError UnknownError { get; } = new ValidationError
        {
            Code = "unknown_error",
            Message = "Unknown error."
        };

        public static IValidationError InvalidArgumentError { get; } = new ValidationError
        {
            Code = "invalid_argument",
            Message = "Invalid argument."
        };

        public static IValidationError OrgOwnerDestroyError { get; } = new ValidationError
        {
            Code = "org_owner_destroy",
            Message = "This user is an organization owner and cannot be destroyed this way. You need to perform the op by destroying the organization."
        };

        public static IValidationError NoPermission { get; } = new ValidationError
        {
            Code = "no_permission",
            Message = "No permissions to perform a requested operation."
        };
    }
}
