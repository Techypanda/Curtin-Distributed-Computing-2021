using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using XSSApplication.Models;
namespace XSSApplication.Controllers
{
    public class LoginController : ApiController
    {
        /*
         * Purpose: Login Endpoint that will take in username and password and no matter what fail, as we dont really need it to ever succeed. - this endpoint is going to lead to XSS.
         * */
        [Route("api/Login/xss")]
        [HttpPost]
        public HttpResponseMessage LoginXSS(LoginPayload payload) {
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent($"{payload.username}, you have failed authentication!") };
        }
        /*
         * Purpose: Login Endpoint that will take in username and password and no matter what fail, as we dont really need it to ever succeed. - this endpoint is not XSS possible
         * */
        [Route("api/Login")]
        [HttpPost]
        public HttpResponseMessage Login(LoginPayload payload) {
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent($"{HttpUtility.HtmlEncode(payload.username)}, you have failed authentication!") };
        }
    }
}
