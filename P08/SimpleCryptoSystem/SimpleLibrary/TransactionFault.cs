using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace SimpleLibrary {
    [DataContract]
    public class TransactionFault {
        private string operation;
        private string reason;
        [DataMember]
        public string Operation {
            get { return operation; }
            set { operation = value; }
        }

        [DataMember]
        public string Reason {
            get { return reason; }
            set { reason = value; }
        }
    }
}
