using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Webservice_Requests;
using Webservice_Responses;
using RestSharp;
using Newtonsoft.Json;
using Business_Webservice.Models;
namespace Business_Webservice.Controllers
{
    public class BankController : ApiController
    {
        private const string apiURI = "https://localhost:44378/";
        
        private void SaveDB() {
            var client = new RestClient(apiURI);
            RestRequest initialRequest = new RestRequest("api/Admin/Save");
            client.Post(initialRequest);
            LogFile.Log("Saved Database Successfully");
        }
        private void ProcessTransactions()
        {
            var client = new RestClient(apiURI);
            RestRequest request = new RestRequest("api/Admin/ProcessTransactions");
            IRestResponse resp = client.Post(request);
            if (resp.StatusCode != HttpStatusCode.OK)
            {
                LogFile.Log($"Failed To Process Transactions! - {resp.Content}");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(resp.Content) });
            }
            LogFile.Log("Successfully Process Transactions!");
        }

        [Route("api/Bank/FetchUser/{userid}")]
        [HttpGet]
        public UserDetailsResponse FetchUser(uint userid)
        {
            var client = new RestClient(apiURI);
            RestRequest request = new RestRequest($"api/UserAccess/Username/{userid}");
            IRestResponse resp = client.Get(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<UserDetailsResponse>(resp.Content);
            } else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(resp.Content) });
            }
        }

        [Route("api/Bank/RegisterAccount")]
        [HttpPost]
        public AccountCreationResponse Register(BusinessAccountCreation payload)
        {
            string encodedFirstname = HttpUtility.HtmlEncode(payload.firstname);
            string encodedLastname = HttpUtility.HtmlEncode(payload.lastname);
            LogFile.Log($"Attempting To Register A New User - {payload.firstname} {payload.lastname}");
            AccountCreationResponse resp = new AccountCreationResponse();
            var client = new RestClient(apiURI);
            RestRequest initialRequest = new RestRequest("api/UserAccess/Create");
            IRestResponse accCreate = client.Post(initialRequest);
            UserCreateResponse content = JsonConvert.DeserializeObject<UserCreateResponse>(accCreate.Content);
            if (accCreate.StatusCode == HttpStatusCode.OK)
            {
                UserSetPayload newPayload = new UserSetPayload() { userID = content.user, firstname = encodedFirstname, lastname = encodedLastname };
                RestRequest finalRequest = new RestRequest("api/UserAccess/SetUsername");
                finalRequest.AddJsonBody(newPayload);
                IRestResponse accNamed = client.Post(finalRequest);
                if (accNamed.StatusCode == HttpStatusCode.OK)
                {
                    LogFile.Log($"Successfully Created A New Account With ID - {content.user}");
                    SaveDB();
                    resp.accountID = content.user;
                    return resp;
                } else
                {
                    LogFile.Log($"Failed To Create New User - {accNamed.Content}");
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(accNamed.Content) });
                }
            } else
            {
                LogFile.Log($"Failed To Create New User - {accCreate.Content}");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(accCreate.Content) });
            }
        }

        [Route("api/Bank/OpenAccount")]
        [HttpPost]
        public AccountCreationResponse OpenAccount(AccountCreationPayload payload)
        {
            LogFile.Log($"Opening A New Bank Account For User ID - {payload.userID}");
            var client = new RestClient(apiURI);
            RestRequest request = new RestRequest("api/AccountAccess/CreateAccount");
            request.AddJsonBody(payload);
            IRestResponse response = client.Post(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                AccountCreationResponse decodedResponse = JsonConvert.DeserializeObject<AccountCreationResponse>(response.Content);
                LogFile.Log($"Successfully Created A New Bank Account ID: {decodedResponse.accountID}");
                SaveDB();
                return decodedResponse;
            } else
            {
                LogFile.Log($"Failed To Create a New Bank Account - {response.Content}");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(response.Content) });
            }
        }

        [Route("api/Bank/Deposit")]
        [HttpPost]
        public HttpResponseMessage Deposit(MoneyRequestPayload payload)
        {
            LogFile.Log($"Depositing Money - {payload.accountID} ${payload.amount}");
            var client = new RestClient(apiURI);
            RestRequest request = new RestRequest("api/AccountAccess/Deposit");
            request.AddJsonBody(payload);
            IRestResponse resp = client.Post(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                LogFile.Log("Successfully Deposited");
                SaveDB();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(resp.Content) };
            } else
            {
                LogFile.Log($"Failed To Deposit $$$ - {resp.Content}");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(resp.Content) });
            }
        }

        [Route("api/Bank/Withdraw")]
        [HttpPost]
        public HttpResponseMessage Withdraw(MoneyRequestPayload payload)
        {
            LogFile.Log($"Withdrawing Money - {payload.accountID} ${payload.amount}");
            var client = new RestClient(apiURI);
            RestRequest req = new RestRequest("api/AccountAccess/Withdraw");
            req.AddJsonBody(payload);
            IRestResponse resp = client.Post(req);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                LogFile.Log("Successfully Withdrawn");
                SaveDB();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(resp.Content) };
            } else
            {
                LogFile.Log($"Failed To Withdraw $$$ - {resp.Content}");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(resp.Content) });
            }
        }

        [Route("api/Bank/Balance")]
        [HttpPost]
        public AccountBalanceResponse Balance(BalanceRequest payload)
        {
            LogFile.Log($"Checking Balance - {payload.accountID}");
            RestClient client = new RestClient(apiURI);
            RestRequest req = new RestRequest($"api/AccountAccess/Balance/{payload.accountID}");
            IRestResponse resp = client.Get(req);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                LogFile.Log("Successfully Checked Balance");
                return JsonConvert.DeserializeObject<AccountBalanceResponse>(resp.Content);
            } else
            {
                LogFile.Log($"Failed To Check Balance - {resp.Content}");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(resp.Content) });
            }
        }

        [Route("api/Bank/SendMoney")]
        [HttpPost]
        public HttpResponseMessage SendMoney(SendMoneyPayload payload)
        {
            LogFile.Log($"Sending Money {payload.myAccountID} -> {payload.recieverAccountID}, ${payload.amount}");
            RestClient client = new RestClient(apiURI);
            RestRequest transCreateRequest = new RestRequest("api/TransactionAccess/Create");
            IRestResponse transResp = client.Post(transCreateRequest);
            if (transResp.StatusCode == HttpStatusCode.OK)
            {
                TransactionCreateResponse resp = JsonConvert.DeserializeObject<TransactionCreateResponse>(transResp.Content);
                try
                {
                    // SendMoneyFromTo(client, payload.myAccountID, payload.senderAccountID, payload.amount);
                    TransactionFromTo(client, payload.myAccountID, payload.recieverAccountID, payload.amount, resp.transID);
                    try
                    {
                        ProcessTransactions();
                    } catch (HttpResponseException e)
                    {
                        LogFile.Log($"Failed To Send Money {e.Response}");
                        throw e;
                    }
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent($"The Money has been send from account {payload.myAccountID} to account {payload.recieverAccountID}.") };
                } catch (HttpResponseException e)
                {
                    LogFile.Log($"Failed To Send Money {e.Response}");
                    throw e;
                }
            } else
            {
                LogFile.Log($"Failed To Send Money {transResp.Content}");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(transResp.Content) });
            }
        }

        private void TransactionFromTo(RestClient client, uint fromID, uint toID, uint currency, uint transID)
        {
            RestRequest setSender = new RestRequest("api/TransactionAccess/SetSender");
            TransactionSenderPayload sendPayload = new TransactionSenderPayload() { senderID = fromID, transID = transID };
            setSender.AddJsonBody(sendPayload);
            IRestResponse SenderResp = client.Post(setSender);
            if (SenderResp.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(SenderResp.Content) });
            }
            RestRequest setReciever = new RestRequest("api/TransactionAccess/SetReciever");
            TransactionRecieverPayload recievePayload = new TransactionRecieverPayload() { transID = transID, recieverID = toID };
            setReciever.AddJsonBody(recievePayload);
            IRestResponse recieverResp = client.Post(setReciever);
            if (recieverResp.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(recieverResp.Content) });
            }
            RestRequest setAmount = new RestRequest("api/TransactionAccess/SetAmount");
            TransactionAmountPayload amountPayload = new TransactionAmountPayload() { transID = transID, amount = currency };
            setAmount.AddJsonBody(amountPayload);
            IRestResponse amountResp = client.Post(setAmount);
            if (amountResp.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(amountResp.Content) });
            }
        }
        private void SendMoneyFromTo(RestClient client, uint fromID, uint toID, uint currency)
        {
            RestRequest bankWithdrawRequest = new RestRequest("api/AccountAccess/Withdraw");
            MoneyRequestPayload payload = new MoneyRequestPayload() { accountID = fromID, amount = currency };
            bankWithdrawRequest.AddJsonBody(payload);
            IRestResponse wResponse = client.Post(bankWithdrawRequest);
            if (wResponse.StatusCode == HttpStatusCode.OK)
            {
                RestRequest bankDeposit = new RestRequest("api/AccountAccess/Deposit");
                MoneyRequestPayload depPayload = new MoneyRequestPayload() { accountID = toID, amount = currency };
                bankDeposit.AddJsonBody(depPayload);
                IRestResponse dResponse = client.Post(bankDeposit);
                if (dResponse.StatusCode == HttpStatusCode.OK)
                {
                    return;
                } else
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(dResponse.Content) });
                }
            } else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(wResponse.Content) });
            }
        }
    }
}
