using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleLibrary {
    public class P2PChainException : Exception {
        public P2PChainException() { }
        public P2PChainException(string mesg) : base(mesg) { }
        public P2PChainException(string msg, Exception inner) : base(msg, inner) { }
    }
}