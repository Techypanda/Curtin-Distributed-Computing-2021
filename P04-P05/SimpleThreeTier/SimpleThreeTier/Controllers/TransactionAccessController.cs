using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BankDB;
using SimpleThreeTier.Models;
using Webservice_Requests;
using Webservice_Responses;
namespace SimpleThreeTier.Controllers
{
    public class TransactionAccessController : ApiController
    {
        private TransactionAccessInterface trans;
        public TransactionAccessController()
        {
            trans = Database.bankDB.GetTransactionInterface();
        }

        [Route("api/TransactionAccess/Create")]
        [HttpPost]
        public TransactionCreateResponse CreateTransaction()
        {
            try
            {
                return new TransactionCreateResponse() { transID = trans.CreateTransaction() };
            } catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/TransactionAccess/Amount/{transID}")]
        [HttpGet]
        public TransacationAmountResponse GetAmount(uint transID)
        {
            try
            {
                trans.SelectTransaction(transID);
                return new TransacationAmountResponse() { amount = trans.GetAmount() };
            } catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/TransactionAccess/SenderAccount/{transID}")]
        [HttpGet]
        public TransactionSenderResponse GetSender(uint transID)
        {
            try
            {
                trans.SelectTransaction(transID);
                return new TransactionSenderResponse() { senderAccount = trans.GetSendrAcct() };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/TransactionAccess/RecieverAccount/{transID}")]
        [HttpGet]
        public TransactionRecieverResponse GetReciever(uint transID)
        {
            try
            {
                trans.SelectTransaction(transID);
                return new TransactionRecieverResponse() { recieverAccount = trans.GetRecvrAcct() };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/TransactionAccess/SetAmount")]
        [HttpPost]
        public HttpResponseMessage SetAmount(TransactionAmountPayload payload)
        {
            try
            {
                trans.SelectTransaction(payload.transID);
                trans.SetAmount(payload.amount);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Successfully Set Amount") };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/TransactionAccess/SetSender")]
        [HttpPost]
        public HttpResponseMessage SetSender(TransactionSenderPayload payload)
        {
            try
            {
                trans.SelectTransaction(payload.transID);
                trans.SetSendr(payload.senderID);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Successfully Set Sender") };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/TransactionAccess/SetReciever")]
        [HttpPost]
        public HttpResponseMessage SetReciever(TransactionRecieverPayload payload)
        {
            try
            {
                trans.SelectTransaction(payload.transID);
                trans.SetRecvr(payload.recieverID);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Successfully Set Reciever") };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }
    }
}
