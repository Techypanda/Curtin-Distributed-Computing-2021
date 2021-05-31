using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRemoteServer {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Simple Remote Server starting...");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            host = new ServiceHost(typeof(DataServer));
            host.AddServiceEndpoint(typeof(DataServerInterface), tcp, "net.tcp://0.0.0.0:8100/DataService");
            host.Open();
            Console.WriteLine("Simple Remote Server is now available on port 8100");
            Console.ReadLine();
            host.Close();
        }
    }
}
