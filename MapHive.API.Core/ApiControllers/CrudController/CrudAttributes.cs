using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MapHive.API.Core.Controllers
{
    /// <summary>
    /// Used to enforce Read privilege checkup for a resource
    /// </summary>
    public class CrudPrivilegeRequiredRead : ActionFilterAttribute
    {
    }

    /// <summary>
    /// Used to enforce Create privilege checkup for a resource
    /// </summary>
    public class CrudPrivilegeRequiredCreate : ActionFilterAttribute
    {
    }

    /// <summary>
    /// Used to enforce Update privilege checkup for a resource
    /// </summary>
    public class CrudPrivilegeRequiredUpdate : ActionFilterAttribute
    {
    }

    /// <summary>
    /// Used to enforce Destroy privilege checkup for a resource
    /// </summary>
    public class CrudPrivilegeRequiredDestroy : ActionFilterAttribute
    {
    }
}
