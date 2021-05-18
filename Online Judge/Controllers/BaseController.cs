using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Online_Judge.Models.Auth;

namespace Online_Judge.Controllers
{
    public abstract class BaseController : Controller
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            byte[] result;
            filterContext.HttpContext.Session.TryGetValue("CurrentUser", out result);
            //如果当前用户未登录，则重定向至登录页面
            if (result == null)
            {
                TempData["Message"] = "请先登录";
                filterContext.Result = new RedirectResult("/Auth/Login");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

       


    }
}
