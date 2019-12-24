using MapHive.Core.DataModel.Validation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Cartomatic.Utils;
using Serilog;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Handles exception using a customised handler
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="exceptionHandler"></param>
        /// <returns></returns>
        protected virtual IActionResult HandleException(Exception ex, Func<Exception, IActionResult> exceptionHandler)
        {
            return HandleException(ex, new[] { exceptionHandler });
        }

        /// <summary>
        /// Standardised exception handler with an option to pass customised handlers. Uses the DefaultExceptionHandlers
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="exceptionHandlers"></param>
        /// <returns></returns>
        protected virtual IActionResult HandleException(Exception ex, IEnumerable<Func<Exception, IActionResult>> exceptionHandlers = null)
        {
            IActionResult handled = null;
            foreach (var handler in exceptionHandlers ?? DefaultExceptionHandlers)
            {
                handled = handler(ex);
                if (handled != null)
                    break;
            }
            return handled;
        }

        /// <summary>
        /// Default exception handlers
        /// </summary>
        protected virtual IEnumerable<Func<Exception, IActionResult>> DefaultExceptionHandlers
            => new List<Func<Exception, IActionResult>>()
            {
                //model validation exceptions
                (e) =>
                {
                    if (e is ValidationFailedException && ((ValidationFailedException)e).ValidationErrors.Any())
                        return new ObjectResult(((ValidationFailedException)e).ValidationErrors){StatusCode = (int)HttpStatusCode.BadRequest};

                    return null;
                },

                //all the unfiltered end up as 500
                (e) =>
                {
                    Log.Error(e, e.Message);

                    //custom exception logger
                    Cartomatic.Utils.Logging.LogExceptions(e);

                    //rollbar too, if enabled & configured
                    if (CommonSettings.Get<bool?>(nameof(ApiConfigurationSettings.EnableRollbarLogging)) == true)
                        Cartomatic.Utils.Logging.LogToRollbar(e);

                    #if DEBUG
                    return new ObjectResult(e.Message){StatusCode = (int)HttpStatusCode.InternalServerError};
                    #endif
                    return new ObjectResult(string.Empty){StatusCode = (int)HttpStatusCode.InternalServerError};
                }
            };

    }
}
