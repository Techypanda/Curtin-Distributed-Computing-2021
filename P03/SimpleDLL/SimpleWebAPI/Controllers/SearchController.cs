using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SimpleWebAPI.Models;
using System.ServiceModel;
using SimpleBusinessTier;

namespace SimpleWebAPI.Controllers
{
    public class SearchController : ApiController
    {
        DataModel dm;
        public SearchController()
        {
            dm = new DataModel();
        }
        // POST: api/Search
        public DataInterm Post(SearchData search)
        {
            try
            {
                return dm.SearchLastName(search.searchStr);
            } catch (FaultException<SearchFault> e)
            {
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent($"{e.Detail.Operation}: {e.Detail.ProblemType}")
                };
                throw new HttpResponseException(resp);
            } catch (NullReferenceException e)
            {
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent($"Search was not received in payload.")
                };
                throw new HttpResponseException(resp);
            }
        }
    }
}
