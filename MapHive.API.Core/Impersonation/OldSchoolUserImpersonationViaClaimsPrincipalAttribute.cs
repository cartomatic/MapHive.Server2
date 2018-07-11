using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MapHive.Api.Core.Filters
{
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
