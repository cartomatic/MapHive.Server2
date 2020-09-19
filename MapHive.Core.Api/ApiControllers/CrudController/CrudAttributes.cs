using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Api.ApiControllers
{

    public class CrudPrivilegeRequired : ActionFilterAttribute
    {
        public CrudPrivilegeRequired()
        {
        }

        protected string PrivCheckupFnName { get; set; }

        /// <summary>
        /// Custom method to be called when executing a priv checkup. It gets executed prior to the default checkup logic;
        /// if it returns true, then the remaining checkup is skipped. method should return a Task{bool} and takes in a dbCtx as a parameter;
        /// It is also possible to provide a controller level implementations for methods called:
        /// IsCrudPrivilegeGrantedForReadAsync
        /// IsCrudPrivilegeGrantedForCreateAsync
        /// IsCrudPrivilegeGrantedForUpdateAsync
        /// IsCrudPrivilegeGrantedForDestroyAsync
        /// </summary>
        /// <param name="privCheckupFnName"></param>
        public CrudPrivilegeRequired(string privCheckupFnName)
        {
            PrivCheckupFnName = privCheckupFnName;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cType = context.Controller.GetType();
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            //try to get db context
            DbContext dbCtx = null;

            
            var getOrganizationDbContextMethodInfo = cType.GetMethods(bindingFlags).FirstOrDefault(m => m.Name == "GetOrganizationDbContext");
            if (getOrganizationDbContextMethodInfo != null)
            {
                try
                {
                    dbCtx = (DbContext) getOrganizationDbContextMethodInfo.Invoke(context.Controller,
                        new object[] {null});
                }
                catch
                {
                    //ignore
                }
            }
            
            if(dbCtx == null)
            {
                var getDefaultDbContextMethodInfo = cType.GetMethods(bindingFlags).FirstOrDefault(m=>m.Name == "GetDefaultDbContext");

                if (getDefaultDbContextMethodInfo != null)
                {
                    dbCtx = (DbContext)getDefaultDbContextMethodInfo.Invoke(context.Controller, null);
                }
            }

            //could not retrieve db context, so perhaps attribute on an invalid object
            if (dbCtx == null)
            {
                await base.OnActionExecutionAsync(context, next);
                return;
            }
                
            var privCheckupFnName = string.Empty;
            var privName = string.Empty;
            MethodInfo privCheckupFn = null;

            if (this is CrudPrivilegeRequiredRead)
            {
                privCheckupFnName = "IsCrudPrivilegeGrantedForReadAsync";
                privName = "R (read)";
            }
            else if (this is CrudPrivilegeRequiredCreate)
            {
                privCheckupFnName = "IsCrudPrivilegeGrantedForCreateAsync";
                privName = "C (create)";
            }
            else if (this is CrudPrivilegeRequiredUpdate)
            {
                privCheckupFnName = "IsCrudPrivilegeGrantedForUpdateAsync";
                privName = "U (update)";
            }
            else if (this is CrudPrivilegeRequiredDestroy)
            {
                privCheckupFnName = "IsCrudPrivilegeGrantedForDestroyAsync";
                privName = "D (destroy)";
            }

            if (!string.IsNullOrEmpty(PrivCheckupFnName))
                privCheckupFnName = PrivCheckupFnName;


            if (!string.IsNullOrWhiteSpace(privCheckupFnName))
            {
                privCheckupFn = cType.GetMethod(privCheckupFnName, bindingFlags);
            }


            if (privCheckupFn != null)
            {
                var privsOk = await (Task<bool>) privCheckupFn.Invoke(context.Controller, new[] {dbCtx});
                if (!privsOk)
                {
                    context.Result = new ObjectResult(
                        new
                        {
                            ErrorMessage = $"No {privName} privilege granted for the requested resource / action"
                        }
                    )
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                    return;
                }
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }

    /// <summary>
    /// Used to enforce Read privilege checkup for a resource
    /// </summary>
    public class CrudPrivilegeRequiredRead : CrudPrivilegeRequired
    {
        public CrudPrivilegeRequiredRead()
        {
        }

        public CrudPrivilegeRequiredRead(string privCheckupFnName)
            : base(privCheckupFnName)
        {
        }
    }

    /// <summary>
    /// Used to enforce Create privilege checkup for a resource
    /// </summary>
    public class CrudPrivilegeRequiredCreate : CrudPrivilegeRequired
    {
        public CrudPrivilegeRequiredCreate()
        {
        }

        public CrudPrivilegeRequiredCreate(string privCheckupFnName)
            : base(privCheckupFnName)
        {
        }
    }

    /// <summary>
    /// Used to enforce Update privilege checkup for a resource
    /// </summary>
    public class CrudPrivilegeRequiredUpdate : CrudPrivilegeRequired
    {
        public CrudPrivilegeRequiredUpdate()
        {
        }

        public CrudPrivilegeRequiredUpdate(string privCheckupFnName)
            : base(privCheckupFnName)
        {
        }
    }

    /// <summary>
    /// Used to enforce Destroy privilege checkup for a resource
    /// </summary>
    public class CrudPrivilegeRequiredDestroy : CrudPrivilegeRequired
    {
        public CrudPrivilegeRequiredDestroy()
        {
        }

        public CrudPrivilegeRequiredDestroy(string privCheckupFnName)
            : base(privCheckupFnName)
        {
        }
    }

}
