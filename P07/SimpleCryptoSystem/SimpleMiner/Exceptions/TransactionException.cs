using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleMiner.Exceptions {
    public class TransactionException : Exception {
        public TransactionException() { }
        public TransactionException(string mesg) : base(mesg) { }
        public TransactionException(string msg, Exception inner) : base(msg, inner) { }
    }
}