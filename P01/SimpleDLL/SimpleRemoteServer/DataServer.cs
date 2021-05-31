using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using SimpleDLL;

namespace SimpleRemoteServer {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class DataServer: DataServerInterface {
        private Database db;
        public DataServer() {
            db = new Database();
        }
        public int GetNumEntries() {
            return db.GetNumRecords();
        }
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName) {
            acctNo = db.GetAcctNoByIndex(index);
            pin = db.GetPINByIndex(index);
            bal = db.GetBalanceByIndex(index);
            fName = db.GetFirstNameByIndex(index);
            lName = db.GetLastNameByIndex(index);
        }
    }
}
