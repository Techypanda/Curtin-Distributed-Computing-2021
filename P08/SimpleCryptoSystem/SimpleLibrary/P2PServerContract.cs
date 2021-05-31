using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace SimpleLibrary {
    [ServiceContract]
    public interface P2PServerContract {
        [OperationContract]
        List<Block> GetCurrentBlockchain();

        [OperationContract]
        Block GetCurrentBlock();

        [OperationContract]
        void RecieveTransaction(Transaction transaction);
    }
}
