using System.Threading.Tasks;
using MapHive.Core.DataModel.Validation;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Return 400 with no permission message error
        /// </summary>
        /// <returns></returns>
        protected static IActionResult NoPermissionError()
        {
            throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("role", ValidationErrors.NoPermission);
        }
    }
}
