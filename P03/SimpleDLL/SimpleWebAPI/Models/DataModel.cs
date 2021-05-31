using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using SimpleDataInterface;
using SimpleBusinessTier;
using System.ServiceModel;
using System.Drawing;
using System.Drawing.Imaging;

namespace SimpleWebAPI.Models
{
    public class DataModel
    {
        private BusinessServerInterface Connect() {
            ChannelFactory<BusinessServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            tcp.OpenTimeout = new TimeSpan(0, 30, 0);
            tcp.CloseTimeout = new TimeSpan(0, 30, 0);
            tcp.SendTimeout = new TimeSpan(0, 30, 0);
            tcp.ReceiveTimeout = new TimeSpan(0, 30, 0);
            string URL = "net.tcp://localhost:8200/BusinessService";
            foobFactory = new ChannelFactory<BusinessServerInterface>(tcp, URL);
            return foobFactory.CreateChannel();
        }
        public DataInterm SearchLastName(string searchString)
        {
            DataInterm temp = new DataInterm();
            Bitmap tempTemp;
            Connect().SearchLastName(searchString, out temp.acct, out temp.pin, out temp.bal, out temp.fname, out temp.lname, out tempTemp);
            MemoryStream byteMemoryStream = new MemoryStream();
            tempTemp.Save(byteMemoryStream, ImageFormat.Jpeg);
            temp.bitmap = Convert.ToBase64String(byteMemoryStream.ToArray());
            return temp;
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap bitmap)
        {
            Connect().GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out bitmap);
        }

        public int GetNumEntries()
        {
            return Connect().GetNumEntries();
        }
    }
}