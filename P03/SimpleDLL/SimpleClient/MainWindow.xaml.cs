using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Text;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using RestSharp;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SimpleBusinessTier;
using System.Threading;
using System.Drawing;
using System.Windows.Interop;

namespace SimpleClient {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        // Put it on the dispatcher
        private delegate APIReturnClass DoSearch(string searchString);
        private delegate IRestResponse RestGetByID(int id);

        private class APIReturnClass
        {
            public int bal;
            public uint acct;
            public uint pin;
            public string fname;
            public string lname;
            public string bitmap;
        }
        private class SearchData
        {
            public string searchStr;
        }

        public MainWindow() {
            InitializeComponent();
            BaseURIInput.Text = "https://localhost:44396/api/";
            SearchLoad.IsIndeterminate = true;
        }

        private void ShowError(string errorDetails) {
            ErrorDetailsText.Text = errorDetails;
            ErrorBackground.Visibility = Visibility.Visible;
            ErrorDetailsText.Visibility = Visibility.Visible;
            ErrorDetailsTitle.Visibility = Visibility.Visible;
            ErrorOccuredTitle.Visibility = Visibility.Visible;
            ErrorAcceptButton.Visibility = Visibility.Visible;
        }
        private void HideError() {
            ErrorDetailsText.Text = "";
            ErrorBackground.Visibility = Visibility.Hidden;
            ErrorDetailsText.Visibility = Visibility.Hidden;
            ErrorDetailsTitle.Visibility = Visibility.Hidden;
            ErrorOccuredTitle.Visibility = Visibility.Hidden;
            ErrorAcceptButton.Visibility = Visibility.Hidden;
        }

        private IRestResponse ValuesFetch(int id)
        {
            var baseURL = "";
            Dispatcher.Invoke(() =>
            {
                baseURL = BaseURIInput.Text;
            });
            var client = new RestClient(baseURL);
            var request = new RestRequest($"GetValues/{id}", RestSharp.DataFormat.Json);
            return client.Get(request);
        }
        private void FinishValuesSearch(IAsyncResult asyncResult)
        {
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            if (asyncObj.EndInvokeCalled == false)
            {
                RestGetByID search;
                search = (RestGetByID)asyncObj.AsyncDelegate;
                try
                {
                    IRestResponse result = search.EndInvoke(asyncObj);
                    APIReturnClass temp = JsonConvert.DeserializeObject<APIReturnClass>(result.Content);
                    Dispatcher.Invoke(() =>
                    {
                        FirstNameBox.Text = temp.fname;
                        LastNameBox.Text = temp.lname;
                        BalanceBox.Text = temp.bal.ToString("C");
                        AcctNoBox.Text = temp.acct.ToString();
                        PinBox.Text = temp.pin.ToString("D4");
                        byte[] binaryData = Convert.FromBase64String(temp.bitmap);
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = new MemoryStream(binaryData);
                        bi.EndInit();
                        ProfilePicture.Source = bi.Clone();
                        UnlockScreen();
                        SearchLoad.Visibility = Visibility.Hidden;
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
             }
        }
        private void GoButton_Click(object sender, RoutedEventArgs e) {
            int index = 0;
            string fName = "", lName = "";
            int bal = 0;
            Bitmap temp;
            uint acct = 0, pin = 0;
            try {
                index = Int32.Parse(IndexBox.Text);
                RestGetByID restRequest = new RestGetByID(this.ValuesFetch);
                AsyncCallback callback = new AsyncCallback(this.FinishValuesSearch);
                SearchLoad.Visibility = Visibility.Visible;
                LockScreen();
                restRequest.BeginInvoke(index, callback, null);
            }
            catch (ArgumentNullException) {
                ShowError("You have entered a invalid number for the index input, are you sure it is a integer i.e no decimal point in the number?");
            }
            catch (FormatException) {
                ShowError("Sorry, it looks like the input you entered for index is not understandable.");
            }
            catch (OverflowException) {
                ShowError("That number input for index is too big.");
            }
        }

        private void ErrorAcceptButton_Click(object sender, RoutedEventArgs e) {
            HideError();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) {
            AsyncCallback callback = new AsyncCallback(this.finishSearchCallback);
            string valToSearch = LastnameSearchVal.Text;
            SearchLoad.Visibility = Visibility.Visible;
            LockScreen();
            DoSearch fetch = new DoSearch(this.Search);
            fetch.BeginInvoke(valToSearch, callback, null);
        }

        private APIReturnClass Search(string searchVal) {
            APIReturnClass retVal = null;
            try {
                var baseURL = "";
                Dispatcher.Invoke(() =>
                {
                    baseURL = BaseURIInput.Text;
                });
                var client = new RestClient(baseURL);
                SearchData sd = new SearchData();
                sd.searchStr = searchVal;
                var request = new RestRequest($"Search", RestSharp.DataFormat.Json);
                request.AddJsonBody(sd);
                var resp = client.Post(request);
                retVal = JsonConvert.DeserializeObject<APIReturnClass>(resp.Content);
            } catch { // Error Occured!
            }
            return retVal;
        }

        private void LockScreen() {
            SearchButton.IsEnabled = false;
            LastnameSearchVal.IsEnabled = false;
            GoButton.IsEnabled = false;
            IndexBox.IsEnabled = false;
        }
        private void UnlockScreen() {
            SearchButton.IsEnabled = true;
            LastnameSearchVal.IsEnabled = true;
            GoButton.IsEnabled = true;
            IndexBox.IsEnabled = true;
        }
        private void finishSearchCallback(IAsyncResult asyncResult) {
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            if (asyncObj.EndInvokeCalled == false) {
                DoSearch search;
                search = (DoSearch)asyncObj.AsyncDelegate;
                APIReturnClass result = search.EndInvoke(asyncObj);
                if (result != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        FirstNameBox.Text = result.fname;
                        LastNameBox.Text = result.lname;
                        BalanceBox.Text = result.bal.ToString("C");
                        AcctNoBox.Text = result.acct.ToString();
                        PinBox.Text = result.pin.ToString("D4");
                        byte[] binaryData = Convert.FromBase64String(result.bitmap);
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = new MemoryStream(binaryData);
                        bi.EndInit();
                        ProfilePicture.Source = bi.Clone();
                    });
                 } else
                 {
                    this.Dispatcher.Invoke(() =>
                    {
                        ShowError("Search Failed");
                    });
                 }
                this.Dispatcher.Invoke(() => {
                    UnlockScreen();
                    SearchLoad.Visibility = Visibility.Hidden;
                });
            }
            asyncObj.AsyncWaitHandle.Close();
        }

        private void AcceptError_Click(object sender, RoutedEventArgs e)
        {
            Error.Visibility = Visibility.Hidden;
        }
    }
}
