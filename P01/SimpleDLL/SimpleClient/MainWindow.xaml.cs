using System;
using System.Collections.Generic;
using System.Linq;
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
using SimpleRemoteServer;

namespace SimpleClient {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private DataServerInterface foob;
        public MainWindow() {
            InitializeComponent();
            ChannelFactory<SimpleRemoteServer.DataServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();
            TotalItemsBox.Text = foob.GetNumEntries().ToString();
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
            uint acct = 0, pin = 0;
            try {
                index = Int32.Parse(IndexBox.Text);
                if (index > foob.GetNumEntries()) {
                    throw new OverflowException();
                } else if (index < 1) {
                    throw new FormatException();
                }
                foob.GetValuesForEntry(index, out acct, out pin, out bal, out fName, out lName);
                FirstNameBox.Text = fName;
                LastNameBox.Text = lName;
                BalanceBox.Text = bal.ToString("C");
                AcctNoBox.Text = acct.ToString();
                PinBox.Text = pin.ToString("D4");
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
    }
}
