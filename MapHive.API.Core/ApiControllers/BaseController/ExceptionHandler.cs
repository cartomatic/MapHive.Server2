using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MapHive.Core.DataModel.Validation;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.API.Core.ApiControllers
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
                        return new NegotiatedContentResult<object>(HttpStatusCode.BadRequest, ((ValidationFailedException)e).ValidationErrors);
                    else return null;
                },

                //all the unfiltered end up as 500
                (e) =>
                {
                    #if DEBUG
                    return new NegotiatedContentResult<object>(HttpStatusCode.InternalServerError, e.Message);
                    #endif
                    return new NegotiatedContentResult<object>(HttpStatusCode.InternalServerError, string.Empty);
                }
            };
    }
}
