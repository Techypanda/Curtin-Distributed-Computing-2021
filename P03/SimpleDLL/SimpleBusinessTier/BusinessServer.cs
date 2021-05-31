using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using SimpleDataInterface;
using System.Drawing;

namespace SimpleBusinessTier {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class BusinessServer : BusinessServerInterface {
        private DataServerInterface connection;
        public BusinessServer() {
            ChannelFactory<DataServerInterface> conFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            tcp.MaxBufferSize = 2147483647;
            tcp.MaxReceivedMessageSize = 2147483647;
            tcp.MaxBufferPoolSize = 214783547;
            string URL = "net.tcp://localhost:8100/DataService";
            conFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            connection = conFactory.CreateChannel();
            Log("A New Connection has been created.");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Log(string logString) {
            Console.WriteLine($"Log {DateTime.Now} - " + logString);
        }

        public int GetNumEntries() {
            Log("GetNumEntries has been called");
            return connection.GetNumEntries();
        }
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap bitmap) {
            Log($"GetValuesForEntry has been called for index: {index}");
            try
            {
                connection.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out bitmap);
            } catch (FaultException<DataServerFault> e)
            {
                Log($"Exception has occured on dataserver for index: {index}\nDetails: {e.Message}");
                DataServerFault dsf = new DataServerFault();
                dsf.Operation = e.Detail.Operation;
                dsf.Reason = e.Detail.Reason;
                throw new FaultException<DataServerFault>(dsf, new FaultReason(e.Message));
            }
        }
        public void SearchLastName(string searchString, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap bitmap) {
            uint acc = 0, pinN = 0;
            Bitmap outBitmap = new Bitmap(100, 100);
            int balN = 0;
            string outFName = "", outLName = "";
            Log($"Search has been begun for searchString: {searchString}");
            for (int i = 0; i < connection.GetNumEntries(); i++) {
                string currentLName;
                connection.GetValuesForEntry(i, out _, out _, out _, out _, out currentLName, out _);
                if (currentLName.ToUpper().Equals(searchString.ToUpper())) {
                    connection.GetValuesForEntry(i, out acc, out pinN, out balN, out outFName, out outLName, out outBitmap);
                    break;
                }
            }

            if (outFName.Equals("") && outLName.Equals("")) {
                SearchFault sf = new SearchFault();
                sf.Operation = "Search Last Name";
                sf.ProblemType = "No Existing User";
                Log($"Search has failed for searchString: {searchString}");
                throw new FaultException<SearchFault>(sf);
            }
            Log($"Search has succeeded for searchString: {searchString}");
            acctNo = acc; pin = pinN; bal = balN; fName = outFName; lName = outLName; bitmap = new Bitmap(outBitmap);
        }
    }
}
