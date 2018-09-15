using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MapHive.Core.Api.Filters
{

    /// <summary>
    /// Used to impersonate a ghost user for the controller action. 
    /// </summary>
    public class ImpersonateGhostUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            Cartomatic.Utils.Identity.ImpersonateGhostUser();
            Cartomatic.Utils.Identity.ImpersonateGhostUserViaHttpContext();
        }
    }
}
