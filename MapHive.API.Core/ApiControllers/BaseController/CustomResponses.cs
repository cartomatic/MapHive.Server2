﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MapHive.Core.DataModel.Validation;

namespace MapHive.Api.Core.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Return 400 with no permission message error
        /// </summary>
        /// <returns></returns>
        public static NegotiatedContentResult<object> NoPermissionError()
        {
            throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("role", ValidationErrors.NoPermission);
        }
    }
}
