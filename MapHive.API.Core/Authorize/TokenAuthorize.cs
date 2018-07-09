using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MapHive.Api.Core.Authorize;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MapHive.Api.Core
{
    /// <summary>
    /// Checks if token auth scheme has been used; if so extracts the token and verifies it agains the remote core api;
    /// Does not skip the user auth so the IsAuthorized output therefore is: base.IsAuthorized() || token is valid (this is so standard user auth can still be used)
    /// </summary>
    public class TokenAuthorizeAttribute : AuthorizeAttribute //, IAuthorizationFilter
    {

        ///// <inheritdoc />
        //public void OnAuthorization(AuthorizationFilterContext context)
        //{
        //    //validate the user first. if user is registered then we're potentially good to go.
        //    var userIsAuthorized = base.IsAuthorized(actionContext);

        //    //extract the token - user can be authorised by the auth api, but the api access may come via token, not the
        //    //standard usr<->org<->apps path
        //    var tokenIsAuthorized = false;
        //    if (actionContext.Request.Headers?.Authorization?.Scheme == TokenAuthorizeMiddleware.AuthScheme)
        //    {
        //        //if api token presence has been detected via middleware
        //        //it should be now present in the request context properties dict
        //        var token = actionContext.Request.Headers.Authorization.Parameter;
        //        tokenIsAuthorized = actionContext.Request.GetOwinContext().Get<bool>(token);

        //        if (tokenIsAuthorized)
        //        {
        //            //impersonate token!
        //            if (Guid.TryParse(token, out var parsedToken))
        //            {
        //                Cartomatic.Utils.Identity.ImpersonateUser(parsedToken);
        //                Cartomatic.Utils.Identity.ImpersonateUserViaHttpContext(parsedToken);
        //            }
        //        }
        //    }

        //    return userIsAuthorized || tokenIsAuthorized;


        //}
    }
}
