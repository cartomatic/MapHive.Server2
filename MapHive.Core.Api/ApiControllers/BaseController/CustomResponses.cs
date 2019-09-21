using System.Net;
using MapHive.Core.DataModel.Validation;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Return 403 with no permission message error
        /// </summary>
        /// <returns></returns>
        protected static IActionResult NoPermissionError()
        {
            return new ObjectResult(ValidationErrors.NoPermission)
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }
    }
}
