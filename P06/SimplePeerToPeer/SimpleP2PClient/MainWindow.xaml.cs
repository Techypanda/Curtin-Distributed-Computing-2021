using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RestSharp;
using Newtonsoft.Json;
using SimpleWebServerResponses;
using SimpleWebServerPayloads;
using System.Runtime.Remoting.Messaging;
using SimpleServerThread;
using System.Threading;
using System.ServiceModel;
using IronPython.Hosting;
using IronPython;
using Microsoft.Scripting.Hosting;
using System.Security.Cryptography;

namespace SimpleP2PClient {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private static Random _random = new Random();
        private string username;
        private delegate void NetworkingThread();
        private delegate void ServerThread();
        private static int jobCount = 0;
        public MainWindow() {
            InitializeComponent();
            ShowRegisterPrompt();
            initServerThread();
            initNetworkingThread();
        }
        private void initNetworkingThread() {
            NetworkingThread nThread = new NetworkingThread(this.CheckClients);
            AsyncCallback nThreadCallback = new AsyncCallback(this.NetworkThreadCallback);
            nThread.BeginInvoke(nThreadCallback, null);
        }
        private void initServerThread() {
            ServerThread sThread = new ServerThread(this.InitializeNetworkServer);
            AsyncCallback sThreadCallback = new AsyncCallback(this.ServerThreadCallback);
            sThread.BeginInvoke(sThreadCallback, null);
        }
        private void InitializeNetworkServer() {
            Console.WriteLine("Simple Network Server Starting...");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            P2PServer serverSingleton = new P2PServer();
            host = new ServiceHost(serverSingleton);
            ((ServiceBehaviorAttribute)host.Description.Behaviors[typeof(ServiceBehaviorAttribute)]).InstanceContextMode = InstanceContextMode.Single;
            tcp.OpenTimeout = new TimeSpan(0, 30, 0);
            tcp.CloseTimeout = new TimeSpan(0, 30, 0);
            tcp.SendTimeout = new TimeSpan(0, 30, 0);
            tcp.ReceiveTimeout = new TimeSpan(0, 30, 0);
            host.AddServiceEndpoint(typeof(P2PServerContract), tcp, "net.tcp://0.0.0.0:8100/P2PServer");
            host.Open(); // Never Closed.
            Console.WriteLine("Simple Remote Server is now available on port 8100");
            Register(IPAddress.Any.ToString(), 8100);
        }
        private void ServerThreadCallback(IAsyncResult asyncResult) {
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            if (asyncObj.EndInvokeCalled == false) {
                ServerThread sThread = (ServerThread)asyncObj.AsyncDelegate;
                try {
                    sThread.EndInvoke(asyncObj);
                    HideRegisterPrompt();
                } catch (Exception e) {
                    PresentError(e.Message);
                }
            }
        }
        private void ShowRegisterPrompt() {
            Dispatcher.Invoke(() => {
                Content.Visibility = Visibility.Hidden;
                RegisterPrompt.Visibility = Visibility.Visible;
            });
        }
        private void HideRegisterPrompt() {
            Dispatcher.Invoke(() => {
                Content.Visibility = Visibility.Visible;
                RegisterPrompt.Visibility = Visibility.Hidden;
            });
        }
        private void Register(string ipAddress, uint port) {
            ipAddress = ipAddress == "0.0.0.0" ? "localhost" : ipAddress; // 0.0.0.0 fix.
            RestClient client = new RestClient("https://localhost:44327/");
            RestRequest req = new RestRequest("api/Connections/Register");
            username = GenerateUniqueSSID();
            RegisterPayload payload = new RegisterPayload() { Username = username, IPAddress = ipAddress, Port = port };
            req.AddJsonBody(payload);
            IRestResponse resp = client.Post(req);
            if (resp.StatusCode == System.Net.HttpStatusCode.OK) {
                Console.WriteLine("Connected");
            } else {
                PresentError(resp.Content);
            }
        }
        private string GenerateUniqueSSID() {
            char[] validChars = "abcdefghijklmnopqstuvwxyz`1234567890-=+/?!@#$%^&*()_+".ToCharArray();
            string outStr = "";
            for (int i = 0; i < _random.Next(0, 9999); i++) {
                outStr += validChars[_random.Next(0, validChars.Length)];
            }
            return outStr;
        }
        private void CheckClients() {
            ChannelFactory<P2PServerContract> conFactory;
            RestClient client = new RestClient("https://localhost:44327/");
            RestRequest req = new RestRequest("api/Connections");
            IRestResponse resp = client.Get(req);
            if (resp.StatusCode == System.Net.HttpStatusCode.OK) {
                GetConnectionsResponse connections = JsonConvert.DeserializeObject<GetConnectionsResponse>(resp.Content);
                try {
                    connections.connections.ForEach((Connection connection) => {
                        NetTcpBinding tcp = new NetTcpBinding();
                        string URL = $"net.tcp://{connection.IPAddress}:{connection.Port}/P2PServer";
                        using (conFactory = new ChannelFactory<P2PServerContract>(tcp, URL)) {
                            P2PServerContract conn = conFactory.CreateChannel();
                            List<Job> jobs = conn.GetJobs();
                            if (jobs.Count > 0) {
                                Loading(true);
                            }
                            jobs.ForEach((Job job) => {
                                SHA256 sha256 = SHA256.Create();
                                byte[] computedHash = sha256.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes(job.PythonCode));
                                if (computedHash.SequenceEqual(job.Hash)) { // Hash is good
                                    try {
                                        ScriptEngine engine = Python.CreateEngine();
                                        ScriptScope scope = engine.CreateScope();
                                        engine.Execute(decode(job.PythonCode), scope);
                                        dynamic pythonFunction = scope.GetVariable("main"); // good python programs have __main__
                                        var res = pythonFunction();
                                        JobResult jResult = new JobResult() { JobID = job.JobID, Result = res };
                                        conn.SubmitJob(jResult);
                                        req = new RestRequest("api/Connections/FinishedJob");
                                        req.AddJsonBody(new FinishJobPayload() { Username = username });
                                        IRestResponse response = client.Post(req);
                                        if (response.StatusCode != HttpStatusCode.OK) {
                                            PresentError(response.Content);
                                        }
                                    } catch (Exception e) {
                                        JobResult jResult = new JobResult() { JobID = job.JobID, Result = $"Python Error: {e.Message}" };
                                        conn.SubmitJob(jResult);
                                        req = new RestRequest("api/Connections/FinishedJob");
                                        req.AddJsonBody(new FinishJobPayload() { Username = username });
                                        IRestResponse response = client.Post(req);
                                        if (response.StatusCode != HttpStatusCode.OK) {
                                            PresentError(response.Content);
                                        }
                                    }
                                    jobCount += 1;
                                }
                            });
                            Loading(false);
                        }
                    });
                } catch (Exception e) {
                    PresentError(e.Message);
                }
            } else {
                throw new Exception(resp.Content);
            }
            Thread.Sleep(50);
        }
        private void Loading(bool status) {
            Dispatcher.Invoke(() => {
                JobsRunningProgress.IsIndeterminate = status;
            });
        }
        private void NetworkThreadCallback(IAsyncResult asyncResult) {
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            if (asyncObj.EndInvokeCalled == false) {
                NetworkingThread nThread = (NetworkingThread)asyncObj.AsyncDelegate;
                try {
                    nThread.EndInvoke(asyncObj);
                    Dispatcher.Invoke(() => {
                        JobsCompletedLabel.Text = $"Jobs Completed: {jobCount}";
                    });
                    //Console.WriteLine(connections);
                    initNetworkingThread();
                } catch (Exception e) {
                    PresentError(e.Message);
                }
            }
        } 
        private void SendClientsToServerThread(List<Connection> connections) {

        }
        private void PresentError(string error) {
            MessageBox.Show("ERROR: " + error);
        }
        private string decode(string text) {
            if (string.IsNullOrEmpty(text)) {
                return text;
            }
            byte[] bytes = Convert.FromBase64String(text);
            return System.Text.UTF8Encoding.UTF8.GetString(bytes);
        }
        private string encode(string text) {
            if (string.IsNullOrEmpty(text)) {
                return text;
            }
            byte[] bytes = System.Text.UTF8Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }
        private void RunCodeButton_Click(object sender, RoutedEventArgs e) {
            string pythonCode = (new TextRange(PythonCodeInput.Document.ContentStart, PythonCodeInput.Document.ContentEnd)).Text;
            SHA256 sha256Hash = SHA256.Create();
            string base64Python = encode(pythonCode);
            uint jobIdd = (uint)_random.Next(0, int.MaxValue);
            Job newJob = new Job() { Hash = sha256Hash.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes(base64Python)), PythonCode = base64Python, JobID = jobIdd };
            P2PServer.Jobs.Add(jobIdd, newJob);
        }

        private void UploadFileButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> open = dlg.ShowDialog();
            if (open == true) {
                PythonCodeInput.Document.Blocks.Clear();
                System.IO.StreamReader file = new System.IO.StreamReader(dlg.FileName);
                string line;
                int i = 0;
                while ((line = file.ReadLine()) != null) {
                    if (i != 0) 
                        PythonCodeInput.AppendText(System.Environment.NewLine + line);
                    else
                        PythonCodeInput.AppendText(line);
                    i += 1;
                }
            }
        }

        private void ViewJobsButton_Click(object sender, RoutedEventArgs e) {
            string jobResults = "--- Completed Jobs ---\r\n";
            P2PServer.CompleteJobs.Values.ToList().ForEach((JobResult res) => {
                jobResults += $"Job {res.JobID} - {res.Result}\r\n";
            });
            MessageBox.Show(jobResults);
        }
    }
}
