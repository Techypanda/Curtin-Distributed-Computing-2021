using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLibrary {
    public class Job {
        public string PythonCode { get; set; }
        public byte[] Hash { get; set; }
        public uint JobID { get; set; }
    }
}
