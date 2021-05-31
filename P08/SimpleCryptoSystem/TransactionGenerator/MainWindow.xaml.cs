using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RestSharp;
using SimpleResponses;
using Newtonsoft.Json;
using SimpleLibrary;
using SimplePayloads;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Net;
using System.Security.Cryptography;

namespace TransactionGenerator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private static Random _random = new Random();
        private string username;
        private uint walletID;
        private delegate void NetworkingThread();
        private delegate void ServerThread();
        private delegate void BlockUpdate();
        private delegate void BalanceUpdate(uint accNo);
        private delegate void TransThread(float amount, uint fromWallet, uint toWallet);
        public MainWindow() {
            InitializeComponent();
            BeginAsyncBlockUpdate();
            initServerThread();
            initNetworkingThread();
            GenerateWalletID();
            DisplayWalletID();
        }

        private void GenerateWalletID() {
            uint myWalletID = 0;
            bool done = false;
            while (!done) {
                float currency = P2PServer._chain.CalculateWalletCoins(myWalletID);
                if (currency == 0) {
                    // Empty Wallet I can take this!
                    done = true;
                } else {
                    myWalletID += 1;
                }
            }
            walletID = myWalletID;
        }

        private void DisplayWalletID() {
            WalletIDEntry.Text = $"My Wallet ID: {walletID}";
            FromWalletInput.Text = $"{walletID}";
        }

        private void initNetworkingThread() {
            NetworkingThread nThread = new NetworkingThread(this.PerformMining);
            AsyncCallback nThreadCallback = new AsyncCallback(this.MiningCallback);
            nThread.BeginInvoke(nThreadCallback, null);
        }
        private void PerformMining() {
            P2PServer._transSemaphore.WaitOne();
            BlockLogic();
            ValidateMyChain();
        }
        private void ValidateMyChain() {
            Dictionary<string, int> popularityDict = new Dictionary<string, int>();
            RestClient client = new RestClient("https://localhost:44307/");
            RestRequest restRequest = new RestRequest("api/Connections");
            IRestResponse resp = client.Get(restRequest);
            if (resp.StatusCode == HttpStatusCode.OK) {
                GetConnectionsResponse decoded = JsonConvert.DeserializeObject<GetConnectionsResponse>(resp.Content);
                decoded.connections.ForEach((Connection con) => { // Build Popularity Dict
                    NetTcpBinding tcp = new NetTcpBinding();
                    string URL = $"net.tcp://{con.IPAddress}:{con.Port}/Blockchain";
                    using (ChannelFactory<P2PServerContract> conFactory = new ChannelFactory<P2PServerContract>(tcp, URL)) {
                        P2PServerContract conn = conFactory.CreateChannel();
                        SimpleLibrary.Block latestBlock = conn.GetCurrentBlock();
                        if (popularityDict.ContainsKey(latestBlock.Hash)) {
                            popularityDict[latestBlock.Hash] += 1;
                        } else {
                            popularityDict.Add(latestBlock.Hash, 1);
                        }
                    }
                });

                List<KeyValuePair<string, int>> popList = popularityDict.ToList();
                popList.Sort(delegate (KeyValuePair<string, int> x, KeyValuePair<string, int> y) {
                    return y.Value.CompareTo(x.Value);
                });
                KeyValuePair<string, int> popularBlock = popList.First(); // Most Popular Block

                if (P2PServer._chain.GetChain().Last().Hash != popularBlock.Key) { // Chain doesnt have the most popular block get rid of it and replace the chain.
                    decoded.connections.ForEach((Connection con) => { // Build Popularity Dict
                        NetTcpBinding tcp = new NetTcpBinding();
                        string URL = $"net.tcp://{con.IPAddress}:{con.Port}/Blockchain";
                        using (ChannelFactory<P2PServerContract> conFactory = new ChannelFactory<P2PServerContract>(tcp, URL)) {
                            P2PServerContract conn = conFactory.CreateChannel();
                            SimpleLibrary.Block latestBlock = conn.GetCurrentBlock();
                            if (latestBlock.Hash == popularBlock.Key) {
                                P2PServer._chain.SetChain(conn.GetCurrentBlockchain()); // swap out the chain
                            }
                        }
                    });
                }
            } else {
                PresentError(resp.Content);
            }
        }
        private void BlockLogic() {
            Transaction tran = P2PServer._transactions.Dequeue();
            SimpleLibrary.Block lastestBlock = P2PServer._chain.GetChain().Last();
            SimpleLibrary.Block newBlock = new SimpleLibrary.Block() { BlockID = lastestBlock.BlockID + 1, Amount = tran.Amount, PreviousHash = lastestBlock.Hash, WalletIDFrom = tran.FromWallet, WalletIDTo = tran.ToWallet };
            GenerateHash(newBlock, out uint offset, out string hash);
            newBlock.BlockOffset = offset;
            newBlock.Hash = hash;
            P2PServer._chain.AddBlock(newBlock);
        }
        private void MiningCallback(IAsyncResult asyncResult) {
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            if (asyncObj.EndInvokeCalled == false) {
                NetworkingThread nThread = (NetworkingThread)asyncObj.AsyncDelegate;
                try {
                    nThread.EndInvoke(asyncObj);
                    initNetworkingThread();
                } catch (Exception e) {
                    PresentError(e.Message);
                }
            }
        }
        private void GenerateHash(SimpleLibrary.Block block, out uint offset, out string hash) {
            SHA256 sha256 = SHA256.Create();
            string bruteForcedHash = "";
            uint tempOffset = 0;
            while (!((bruteForcedHash.StartsWith("12345") /* && bruteForcedHash.EndsWith("54321") */))) {
                string temp = $"{block.WalletIDFrom}{block.WalletIDTo}{block.Amount}{tempOffset}{block.PreviousHash}";
                bruteForcedHash = BitConverter.ToUInt64(sha256.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes(temp)), 0).ToString();
                if (!((bruteForcedHash.StartsWith("12345") /* && bruteForcedHash.EndsWith("54321") */))) {
                    tempOffset += 5;
                }
            }
            offset = tempOffset;
            hash = bruteForcedHash;
        }

        private void initServerThread() {
            ServerThread sThread = new ServerThread(this.InitializeNetworkServer);
            AsyncCallback sThreadCallback = new AsyncCallback(this.ServerThreadCallback);
            sThread.BeginInvoke(sThreadCallback, null);
        }
        private void InitializeNetworkServer() {
            Console.WriteLine("Simple Blockchain Server Starting...");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            P2PServer serverSingleton = new P2PServer();
            host = new ServiceHost(serverSingleton);
            ((ServiceBehaviorAttribute)host.Description.Behaviors[typeof(ServiceBehaviorAttribute)]).InstanceContextMode = InstanceContextMode.Single;
            tcp.OpenTimeout = new TimeSpan(0, 30, 0);
            tcp.CloseTimeout = new TimeSpan(0, 30, 0);
            tcp.SendTimeout = new TimeSpan(0, 30, 0);
            tcp.ReceiveTimeout = new TimeSpan(0, 30, 0);
            host.AddServiceEndpoint(typeof(P2PServerContract), tcp, "net.tcp://0.0.0.0:8100/Blockchain");
            host.Open(); // Never Closed.
            Console.WriteLine("Simple Remote Server is now available on port 8100");
            Register(IPAddress.Any.ToString(), 8100);
        }
        private void Register(string ipAddress, uint port) {
            ipAddress = ipAddress == "0.0.0.0" ? "localhost" : ipAddress; // 0.0.0.0 fix.
            RestClient client = new RestClient("https://localhost:44307/");
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
            for (int i = 0; i < _random.Next(0, 999); i++) {
                outStr += validChars[_random.Next(0, validChars.Length)];
            }
            return outStr;
        }
        private void ServerThreadCallback(IAsyncResult asyncResult) {
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            if (asyncObj.EndInvokeCalled == false) {
                ServerThread sThread = (ServerThread)asyncObj.AsyncDelegate;
                try {
                    sThread.EndInvoke(asyncObj);
                } catch (Exception e) {
                    PresentError(e.Message);
                }
            }
        }


        private void BeginAsyncBlockUpdate() {
            AsyncCallback bThreadCallback = new AsyncCallback(this.UpdateBlocksCallback);
            BlockUpdate bThread = new BlockUpdate(this.UpdateBlocks);
            UpdateBlocksProgress.IsIndeterminate = true;
            bThread.BeginInvoke(bThreadCallback, null);
        }

        private void UpdateBalance() {
            try {
                uint accNo = uint.Parse(AccountNumberInput.Text);
                BalanceProgress.IsIndeterminate = true;
                AsyncCallback bThreadCallback = new AsyncCallback(this.UpdateBalanceCallback);
                BalanceUpdate bThread = new BalanceUpdate(this.PerformBalanceAPI);
                bThread.BeginInvoke(accNo, bThreadCallback, null);
            } catch (FormatException e) {
                PresentError(e.Message);
            }
        }

        private void PerformBalanceAPI(uint accNo) {
            Dispatcher.Invoke(() => {
                BalanceDisplay.Text = $"${P2PServer._chain.CalculateWalletCoins(accNo)}";
            });
        }

        private void UpdateBalanceCallback(IAsyncResult result) {
            AsyncResult asyncResult = (AsyncResult)result;
            if (asyncResult.EndInvokeCalled == false) {
                BalanceUpdate bThread = (BalanceUpdate)asyncResult.AsyncDelegate;
                try {
                    bThread.EndInvoke(asyncResult);
                    Dispatcher.Invoke(() => {
                        BalanceProgress.IsIndeterminate = false;
                    });
                } catch (Exception e) {
                    PresentError(e.Message);
                }
            }
        }

        private void UpdateBlocks() {
            Dispatcher.Invoke(() => {
                BlockchainBlocks.Items.Clear();
            });
            P2PServer._chain.GetChain().ForEach((SimpleLibrary.Block block) => {
                Dispatcher.Invoke(() => {
                    BlockchainBlocks.Items.Add($"ID: {block.BlockID} - Coins: {block.Amount}, Offset: {block.BlockOffset}, Hash: {block.Hash}, Previous Hash: {block.PreviousHash}, Wallet From: {block.WalletIDFrom}, Wallet To: {block.WalletIDTo}");
                });
            });
        }

        private void UpdateBlocksCallback(IAsyncResult result) {
            AsyncResult asyncResult = (AsyncResult)result;
            if (asyncResult.EndInvokeCalled == false) {
                BlockUpdate bThread = (BlockUpdate)asyncResult.AsyncDelegate;
                try {
                    bThread.EndInvoke(asyncResult);
                    Dispatcher.Invoke(() => {
                        UpdateBlocksProgress.IsIndeterminate = false;
                    });
                } catch (Exception e) {
                    PresentError(e.Message);
                }
            }
        }

        private void PresentError(string content) {
            Dispatcher.Invoke(() => {
                MessageBox.Show(content);
            });
        }

        private void UpdateBlocksButton_Click(object sender, RoutedEventArgs e) {
            BeginAsyncBlockUpdate();
        }

        private void CheckBalanceButton_Click(object sender, RoutedEventArgs e) {
            UpdateBalance();
        }

        private void BeginTrans(float amount, uint fromWallet, uint toWallet) {
            Transaction trans = new Transaction { Amount = amount, FromWallet = fromWallet, ToWallet = toWallet };
            RestClient client = new RestClient("https://localhost:44307/");
            RestRequest restRequest = new RestRequest("api/Connections");
            IRestResponse resp = client.Get(restRequest);
            if (resp.StatusCode == HttpStatusCode.OK) {
                GetConnectionsResponse decoded = JsonConvert.DeserializeObject<GetConnectionsResponse>(resp.Content);
                try {
                    decoded.connections.ForEach((Connection con) => { // Build Popularity Dict
                        NetTcpBinding tcp = new NetTcpBinding();
                        string URL = $"net.tcp://{con.IPAddress}:{con.Port}/Blockchain";
                        using (ChannelFactory<P2PServerContract> conFactory = new ChannelFactory<P2PServerContract>(tcp, URL)) {
                            P2PServerContract conn = conFactory.CreateChannel();
                            conn.RecieveTransaction(trans);
                        }
                    });
                } catch (FaultException<TransactionFault> e) {
                    PresentError(e.Detail.Reason);
                }
            } else {
                PresentError(resp.Content);
            }
        }
        private void TransCallback(IAsyncResult result) {
            AsyncResult asyncResult = (AsyncResult)result;
            if (asyncResult.EndInvokeCalled == false) {
                TransThread tThread = (TransThread)asyncResult.AsyncDelegate;
                try {
                    tThread.EndInvoke(asyncResult);
                } catch (Exception e) {
                    PresentError(e.Message);
                }
                Dispatcher.Invoke(() => {
                    TransBar.IsIndeterminate = false;
                });
            }
        }

        private void SendTransButton_Click(object sender, RoutedEventArgs e) {
            try {
                float amount = float.Parse(AmountInput.Text);
                uint fromWallet = uint.Parse(FromWalletInput.Text);
                uint toWallet = uint.Parse(ToWalletInput.Text);
                TransBar.IsIndeterminate = true;
                TransThread tThread = new TransThread(this.BeginTrans);
                AsyncCallback asyncCallback = new AsyncCallback(this.TransCallback);
                tThread.BeginInvoke(amount, fromWallet, toWallet, asyncCallback, null);
            } catch (FormatException) {
                MessageBox.Show("Invalid Input For Transaction");
            }
        }

        private void TransferCheck_Checked(object sender, RoutedEventArgs e) {
            FromWalletInput.Text = $"0";
        }

        private void TransferCheck_Unchecked(object sender, RoutedEventArgs e) {
            FromWalletInput.Text = $"{walletID}";
        }
    }
}
