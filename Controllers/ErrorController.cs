using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_Project.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/GlobalError")]
        public IActionResult GlobalError(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                ViewBag.StatusCode = statusCode.Value;
            }

            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ViewBag.ErrorMessage = exceptionHandlerPathFeature?.Error.Message;

            return View();
        }
    }
}
