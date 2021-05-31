using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlockchainServer.Exceptions {
    public class BlockChainException : Exception {
        public BlockChainException() { }
        public BlockChainException(string mesg) : base(mesg) { }
        public BlockChainException(string msg, Exception inner) : base(msg, inner) { }
    }
}