using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SimpleThreeTier.Models;
using Webservice_Requests;
using Webservice_Responses;
using BankDB;
namespace SimpleThreeTier.Controllers
{
    public class AccountAccessController : ApiController
    {
        private AccountAccessInterface accs;
        public AccountAccessController()
        {
            accs = Database.bankDB.GetAccountInterface();
        }

        [Route("api/AccountAccess/GetUserAccountIDs/{accountID}")]
        [HttpGet]
        public List<uint> GetUserAccountIDs(uint accountID)
        {
            try
            {
                return accs.GetAccountIDsByUser(accountID);
            }
            catch (Exception e) // We dont know the exception type, BankDB doesnt expose if it has exceptions
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/AccountAccess/CreateAccount")]
        [HttpPost]
        public AccountCreationResponse CreateAccount(AccountCreationPayload payload)
        {
            try
            {
                AccountCreationResponse newAccount = new AccountCreationResponse();
                newAccount.accountID = accs.CreateAccount(payload.userID);
                return newAccount;
            }
            catch (Exception e) // We dont know the exception type, BankDB doesnt expose if it has exceptions
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/AccountAccess/Deposit")]
        [HttpPost]
        public HttpResponseMessage Deposit(MoneyRequestPayload payload)
        {
            try
            {
                accs.SelectAccount(payload.accountID);
                accs.Deposit(payload.amount);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent($"Deposited {payload.amount} in {payload.accountID}") };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/AccountAccess/Withdraw")]
        [HttpPost]
        public HttpResponseMessage Withdraw(MoneyRequestPayload payload)
        {
            try
            {
                accs.SelectAccount(payload.accountID);
                accs.Withdraw(payload.amount);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent($"Withdrawn {payload.amount} from {payload.accountID}") };
            } catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/AccountAccess/Balance/{accountID}")]
        [HttpGet]
        public AccountBalanceResponse Balance(uint accountID)
        {
            try
            {
                accs.SelectAccount(accountID);
                return new AccountBalanceResponse() { balance = accs.GetBalance() };
            } catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/AccountAccess/Owner/{accountID}")]
        [HttpGet]
        public AccountOwnerResponse Owner(uint accountID)
        {
            try
            {
                accs.SelectAccount(accountID);
                return new AccountOwnerResponse() { owner = accs.GetOwner() };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/AccountAccess/AccountDetails/{accountID}")]
        [HttpGet]
        public AccountDetailsResponse AccountDetails(uint accountID)
        {
            try
            {
                accs.SelectAccount(accountID);
                return new AccountDetailsResponse() { balance = accs.GetBalance(), owner = accs.GetOwner() };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }
    }
}
