using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webservice_Requests
{
    public class SendMoneyPayload
    {
        public uint recieverAccountID;
        public uint myAccountID;
        public uint amount;
    }
}
