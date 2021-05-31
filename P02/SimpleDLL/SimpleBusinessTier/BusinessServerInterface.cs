using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Drawing;

namespace SimpleBusinessTier {
    [ServiceContract]
    public interface BusinessServerInterface {
        [OperationContract]
        int GetNumEntries();
        [OperationContract]
        void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap bitmap);
        [OperationContract]
        [FaultContract(typeof(SearchFault))]
        void SearchLastName(string searchString, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap bitmap);
    }
}
