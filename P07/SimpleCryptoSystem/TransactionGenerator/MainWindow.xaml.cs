using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using SimpleResponses;
using Newtonsoft.Json;
using SimpleLibrary;
using SimplePayloads;
using System.Runtime.Remoting.Messaging;

namespace TransactionGenerator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private delegate void BlockUpdate();
        private delegate void BalanceUpdate(int accNo);
        private delegate void TransThread(float amount, uint fromWallet, uint toWallet);
        public MainWindow() {
            InitializeComponent();
            BeginAsyncBlockUpdate();
        }

        private void BeginAsyncBlockUpdate() {
            AsyncCallback bThreadCallback = new AsyncCallback(this.UpdateBlocksCallback);
            BlockUpdate bThread = new BlockUpdate(this.UpdateBlocks);
            UpdateBlocksProgress.IsIndeterminate = true;
            bThread.BeginInvoke(bThreadCallback, null);
        }

        private void UpdateBalance() {
            try {
                int accNo = int.Parse(AccountNumberInput.Text);
                BalanceProgress.IsIndeterminate = true;
                AsyncCallback bThreadCallback = new AsyncCallback(this.UpdateBalanceCallback);
                BalanceUpdate bThread = new BalanceUpdate(this.PerformBalanceAPI);
                //UpdateBlocksProgress.IsIndeterminate = true;
                bThread.BeginInvoke(accNo, bThreadCallback, null);
            } catch (FormatException e) {
                PresentError(e.Message);
            }
        }

        private void PerformBalanceAPI(int accNo) {
            RestClient client = new RestClient("https://localhost:44362/");
            RestRequest request = new RestRequest($"api/Crypto/Balance/{accNo}");
            IRestResponse response = client.Get(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                CryptoBalanceResponse apiReturn = JsonConvert.DeserializeObject<CryptoBalanceResponse>(response.Content);
                Dispatcher.Invoke(() => {
                    BalanceDisplay.Text = $"${apiReturn.currency.ToString()}";
                });
            } else {
                PresentError(response.Content);
            }
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
            RestClient client = new RestClient("https://localhost:44362/");
            RestRequest request = new RestRequest("api/Crypto/State");
            IRestResponse response = client.Get(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                CryptoStateResponse state = JsonConvert.DeserializeObject<CryptoStateResponse>(response.Content);
                Dispatcher.Invoke(() => {
                    BlockchainBlocks.Items.Clear();
                });
                state.state.ForEach((SimpleLibrary.Block block) => {
                    Dispatcher.Invoke(() => {
                        BlockchainBlocks.Items.Add($"ID: {block.BlockID} - Coins: {block.Amount}, Offset: {block.BlockOffset}, Hash: {block.Hash}, Previous Hash: {block.PreviousHash}, Wallet From: {block.WalletIDFrom}, Wallet To: {block.WalletIDTo}");
                    });
                });
            } else {
                PresentError(response.Content);
            }
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
            MessageBox.Show(content);
        }

        private void UpdateBlocksButton_Click(object sender, RoutedEventArgs e) {
            BeginAsyncBlockUpdate();
        }

        private void CheckBalanceButton_Click(object sender, RoutedEventArgs e) {
            UpdateBalance();
        }

        private void BeginTrans(float amount, uint fromWallet, uint toWallet) {
            RestClient client = new RestClient("https://localhost:44356/");
            RestRequest request = new RestRequest("api/Mining/AddTransaction");
            request.AddJsonBody(new AddBlockRequest() { amount = amount, toWallet = toWallet, fromWallet = fromWallet });
            IRestResponse response = client.Post(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                PresentError(response.Content);
            }
        }
        private void TransCallback(IAsyncResult result) {
            AsyncResult asyncResult = (AsyncResult)result;
            if (asyncResult.EndInvokeCalled == false) {
                TransThread tThread = (TransThread)asyncResult.AsyncDelegate;
                try {
                    tThread.EndInvoke(asyncResult);
                    Dispatcher.Invoke(() => {
                        TransBar.IsIndeterminate = false;
                    });
                } catch (Exception e) {
                    PresentError(e.Message);
                }
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
    }
}
