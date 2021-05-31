using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
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
        private delegate SearchResults DoSearch(string searchString);

        private class SearchResults {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Balance { get; set; }
            public uint Account { get; set; }
            public uint Pin { get; set; }
            public bool Error { get; set; }
        }

        private BusinessServerInterface foob;
        public MainWindow() {
            InitializeComponent();
            ChannelFactory<BusinessServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            tcp.OpenTimeout = new TimeSpan(0, 30, 0);
            tcp.CloseTimeout = new TimeSpan(0, 30, 0);
            tcp.SendTimeout = new TimeSpan(0, 30, 0);
            tcp.ReceiveTimeout = new TimeSpan(0, 30, 0);
            string URL = "net.tcp://localhost:8200/BusinessService";
            foobFactory = new ChannelFactory<BusinessServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();
            TotalItemsBox.Text = foob.GetNumEntries().ToString();
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

        private void GoButton_Click(object sender, RoutedEventArgs e) {
            int index = 0;
            string fName = "", lName = "";
            int bal = 0;
            Bitmap temp;
            uint acct = 0, pin = 0;
            try {
                index = Int32.Parse(IndexBox.Text);
                if (index > foob.GetNumEntries()) {
                    throw new OverflowException();
                } else if (index < 1) {
                    throw new FormatException();
                }
                foob.GetValuesForEntry(index, out acct, out pin, out bal, out fName, out lName, out temp);
                FirstNameBox.Text = fName;
                LastNameBox.Text = lName;
                BalanceBox.Text = bal.ToString("C");
                AcctNoBox.Text = acct.ToString();
                PinBox.Text = pin.ToString("D4");
                if (temp != null)
                {
                    ProfilePicUpdate(temp);
                }
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

        // Hacky Fix due to WPF
        private void ProfilePicUpdate(Bitmap temp)
        {
            this.Dispatcher.Invoke(() =>
            {
                ImageSourceConverter c = new ImageSourceConverter();
                var img = Imaging.CreateBitmapSourceFromHBitmap(temp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                ProfilePicture.Source = img;
                temp.Dispose();
            });
        }

        private SearchResults Search(string searchVal) {
            Bitmap temp = null;
            string fName = "", lName = "";
            int bal = 0;
            uint acct = 0, pin = 0;
            bool error = true;
            try {
                foob.SearchLastName(searchVal, out acct, out pin, out bal, out fName, out lName, out temp);
                error = false;
            } catch { // Error Occured!
            }
            SearchResults outObj = new SearchResults();
            outObj.FirstName = fName;
            outObj.LastName = lName;
            outObj.Pin = pin;
            outObj.Balance = bal;
            outObj.Account = acct;
            outObj.Error = error;
            if (temp != null)
            {
                ProfilePicUpdate(temp);
            }
            return outObj;
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
                try {
                    SearchResults result = search.EndInvoke(asyncObj);
                    this.Dispatcher.Invoke(() =>
                    {
                        FirstNameBox.Text = result.FirstName;
                        LastNameBox.Text = result.LastName;
                        BalanceBox.Text = result.Balance.ToString("C");
                        AcctNoBox.Text = result.Account.ToString();
                        PinBox.Text = result.Pin.ToString("D4");
                    });
                } catch (FaultException e) { // fix at some point.
                    this.Dispatcher.Invoke(() => {
                        Error.Visibility = Visibility.Visible;
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
