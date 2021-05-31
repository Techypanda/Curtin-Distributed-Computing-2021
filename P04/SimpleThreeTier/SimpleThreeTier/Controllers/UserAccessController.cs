using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Webservice_Requests;
using Webservice_Responses;
using BankDB;
using SimpleThreeTier.Models;
namespace SimpleThreeTier.Controllers
{
    public class UserAccessController : ApiController
    {
        private UserAccessInterface users;
        public UserAccessController()
        {
            users = Database.bankDB.GetUserAccess();
        }

        [Route("api/UserAccess/Users")]
        [HttpGet]
        public UsersResponse Users()
        {
            try
            {
                return new UsersResponse() { users = users.GetUsers() };
            } catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/UserAccess/Create")]
        [HttpPost]
        [HttpGet]
        public UserCreateResponse CreateUser()
        {
            try
            {
                return new UserCreateResponse() { user = users.CreateUser() };
            } catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/UserAccess/Username/{userid}")]
        [HttpGet]
        public UserDetailsResponse GetUsername(uint userid)
        {
            try
            {
                string firstname, lastname;
                users.SelectUser(userid);
                users.GetUserName(out firstname, out lastname);
                return new UserDetailsResponse() { firstname = firstname, lastname = lastname };
            } catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/UserAccess/SetUsername")]
        [HttpPost]
        public HttpResponseMessage SetUsername(UserSetPayload payload)
        {
            try
            {
                users.SelectUser(payload.userID);
                users.SetUserName(payload.firstname, payload.lastname);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Successfully Set Username") };
            } catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }
    }
}
