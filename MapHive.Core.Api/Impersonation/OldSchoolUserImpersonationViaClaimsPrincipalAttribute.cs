using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MapHive.Core.Api.Filters
{

    /// <summary>
    /// Impersonates user the old way; a bit not in line with aspnet core though, but these utils are used in many places and passing identity explicitly doesn't seem to be a viable option. at least at this stage
    /// </summary>
    public class OldSchoolUserImpersonationViaClaimsPrincipalAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var ci = actionContext.HttpContext.User?.Identity as System.Security.Claims.ClaimsIdentity;
            var sub = ci?.Claims.FirstOrDefault(c => c.Type == Cartomatic.Utils.Identity.Subject);

            Cartomatic.Utils.Identity.ImpersonateUser(sub?.Value);
        }
    }

}
