using MapHive.Core.DataModel.Validation;
using System.Web.Http.Results;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Return 400 with no permission message error
        /// </summary>
        /// <returns></returns>
        protected static NegotiatedContentResult<object> NoPermissionError()
        {
            throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("role", ValidationErrors.NoPermission);
        }
    }
}
