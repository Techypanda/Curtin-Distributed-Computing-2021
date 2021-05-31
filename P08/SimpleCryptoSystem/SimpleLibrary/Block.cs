using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleLibrary {
    public class Block {
        public uint BlockID { get; set; }
        public uint WalletIDFrom { get; set; }
        public uint WalletIDTo { get; set; }
        public float Amount { get; set; }
        public uint BlockOffset { get; set; }
        public string PreviousHash { get; set; } 
        public string Hash { get; set; }
    }
}