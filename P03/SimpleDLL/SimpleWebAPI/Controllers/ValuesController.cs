using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SimpleWebAPI.Models;
namespace SimpleWebAPI.Controllers
{
    public class ValuesController : ApiController
    {
        private DataModel dm;
        public ValuesController()
        {
            dm = new DataModel();
        }
        // GET: api/Values
        public int Get()
        {
            return dm.GetNumEntries();
        }
    }
}
