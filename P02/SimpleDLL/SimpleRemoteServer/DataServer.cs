using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using SimpleDLL;
using SimpleDataInterface;
using System.Drawing;

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
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap bitmap) {
            try
            {
                acctNo = db.GetAcctNoByIndex(index);
                pin = db.GetPINByIndex(index);
                bal = db.GetBalanceByIndex(index);
                fName = db.GetFirstNameByIndex(index);
                lName = db.GetLastNameByIndex(index);
                bitmap = db.GetPictureByIndex(index);
            } catch (DatabaseException e)
            {
                DataServerFault dsf = new DataServerFault();
                dsf.Operation = e.Operation;
                dsf.Reason = e.Message;
                throw new FaultException<DataServerFault>(dsf);
            }
        }
    }
}
