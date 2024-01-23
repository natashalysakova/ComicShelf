using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;

namespace ComicShelf.Utilities
{
    public static class ExtentionMethods
    {
        public static StatusCodeResult StatusCode(this PageModel page, HttpStatusCode statusCode)
        {
            return page.StatusCode((int)statusCode);
        }
        public static ObjectResult StatusCode(this PageModel page, HttpStatusCode statusCode, object value)
        {
            return page.StatusCode((int)statusCode, value);
        }
    }
}
