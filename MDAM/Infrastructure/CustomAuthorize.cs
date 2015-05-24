using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MDAM.Controllers;

namespace MDAM.Infrastructure
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            filterContext.Result = new RedirectToRouteResult(
            new RouteValueDictionary(
                new { Action = "AccessDenied", Controller = "Account" }));
        }
    }
}