using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebFrontEnd.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Ambil nilai session login
            var isLoggedIn = context.HttpContext.Session.GetString("IsLoggedIn");

            // Ambil nama controller & action yang sedang diakses
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var actionName = context.RouteData.Values["action"]?.ToString();

            // Lewatkan jika sedang akses Login atau halaman publik
            if (isLoggedIn != "true" && !(controllerName == "Home" && (actionName == "Login" || actionName == "Register" || actionName == "Index")))
            {
                context.Result = new RedirectToActionResult("Login", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
