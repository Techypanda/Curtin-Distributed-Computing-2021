using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDLL {
    internal class DatabaseGenerator {
        private Random rnd;
        public DatabaseGenerator() {
            rnd = new Random();
        }
        public void GetNextAccount(out uint pin, out uint acctNo, out string firstName, out string lastName, out int balance) {
            pin = GetPIN();
            acctNo = GetAcctNo();
            firstName = GetFirstname();
            lastName = GetLastname();
            balance = GetBalance();
        }
        private string GetFirstname() { // name 8 - 30 length
            return GenerateName(rnd.Next(8, 30));
        }
        private string GetLastname() { // name 8 - 30 length
            return GenerateName(rnd.Next(8, 30));
        }
        private uint GetPIN() { // 4 - 12 digits
            return (uint)rnd.Next(4, 13);
        }
        private uint GetAcctNo() { // 8 - 12 digits
            return (uint)rnd.Next(8, 13);
        }
        private int GetBalance() { // 0 - 999999$
            return rnd.Next(0, 100000);
        }
        private string GenerateName(int len) { // https://stackoverflow.com/questions/14687658/random-name-generator-in-c-sharp
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[rnd.Next(consonants.Length)].ToUpper();
            Name += vowels[rnd.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len) {
                Name += consonants[rnd.Next(consonants.Length)];
                b++;
                Name += vowels[rnd.Next(vowels.Length)];
                b++;
            }
            return Name;
        }
    }
}
