using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SimpleThreeTier.Models;
namespace SimpleThreeTier.Controllers
{
    public class AdminController : ApiController
    {
        [Route("api/Admin/Save")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Save()
        {
            Database.bankDB.SaveToDisk();
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Successfully Saved DB") };
        }
        [Route("api/Admin/ProcessTransactions")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage ProcessTransactions()
        {
            try
            {
                Database.bankDB.ProcessAllTransactions();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Successfully Processed DB Transactions") };
            } catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }
    }
}
