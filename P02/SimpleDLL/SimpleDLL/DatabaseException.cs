using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDLL
{
    public class DatabaseException : Exception
    {
        public string Operation { get; set; }
        public DatabaseException()
        {
        }
        public DatabaseException(string message, string op) : base(message)
        {
            Operation = op;
        }
        public DatabaseException(string message, Exception inner)
        {
        }
    }
}
