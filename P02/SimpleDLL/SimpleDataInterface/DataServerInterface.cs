using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Drawing;

namespace SimpleDataInterface {
    [ServiceContract]
    public interface DataServerInterface {
        [OperationContract]
        int GetNumEntries();
        [OperationContract]
        [FaultContract(typeof(DataServerFault))]
        void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap bitmap);
    }
}