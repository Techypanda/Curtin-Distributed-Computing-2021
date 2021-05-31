using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLibrary {
    public class Transaction {
        public float Amount { get; set; }
        public uint FromWallet { get; set; }
        public uint ToWallet { get; set; }
    }
}
