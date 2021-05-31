using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.ServiceModel;
using SimpleWebAPI.Models;
using SimpleBusinessTier;
using SimpleDataInterface;
using System.Drawing;
using System.Drawing.Imaging;

namespace SimpleWebAPI.Controllers
{
    public class GetValuesController : ApiController
    {
        private DataModel dm;
        public GetValuesController()
        {
            dm = new DataModel();
        }
        // GET: api/GetValues/5
        public DataInterm Get(int id)
        {
            try
            {
                DataInterm di = new DataInterm();
                Bitmap tempTemp;
                dm.GetValuesForEntry(id, out di.acct, out di.pin, out di.bal, out di.fname, out di.lname, out tempTemp);
                MemoryStream byteMemoryStream = new MemoryStream();
                tempTemp.Save(byteMemoryStream, ImageFormat.Jpeg);
                di.bitmap = Convert.ToBase64String(byteMemoryStream.ToArray());
                return di;
            } catch (FaultException/*<DataServerFault>*/ e) // Loses The Details over WCF??
            {
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent($"That Index Does Not Exist")
                };
                throw new HttpResponseException(resp);
            }
        }
    }
}
