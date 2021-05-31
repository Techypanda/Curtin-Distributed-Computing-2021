using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SimpleMiner.Models;
using SimplePayloads;
using SimpleMiner.Exceptions;
namespace SimpleMiner.Controllers
{
    public class MiningController : ApiController
    {
        private static TransactionsDB db = new TransactionsDB();
        [Route("api/Mining/AddTransaction")]
        [HttpPost]
        public HttpResponseMessage AddTransaction(AddBlockRequest payload) {
            try {
                db.AddTransaction(payload);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Queued Up") };
            } catch (TransactionException e) {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }
    }
}
