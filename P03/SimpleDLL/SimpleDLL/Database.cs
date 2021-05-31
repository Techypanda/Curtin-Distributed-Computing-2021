using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDLL {
    public class Database {
        List<DatabaseStorage> storage;
        public static Database Instance { get; } = new Database();
        public Database() {
            DatabaseGenerator localGenerator = new DatabaseGenerator();
            storage = new List<DatabaseStorage>();
            for (int i = 0; i < 100000; i++) {
                DatabaseStorage newStorage = new DatabaseStorage();
                localGenerator.GetNextAccount(
                    out newStorage.pin,
                    out newStorage.acctNo,
                    out newStorage.firstName,
                    out newStorage.lastName,
                    out newStorage.balance,
                    out newStorage.picture
                );
                storage.Add(newStorage);
            }
        }
        public uint GetAcctNoByIndex(int index) {
            uint acctNo;
            try
            {
                acctNo = storage[index].acctNo;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new DatabaseException("That Index does not exist", "GetAcctNoByIndex");
            }
            catch (IndexOutOfRangeException e)
            {
                throw new DatabaseException("That Index does not exist", "GetAcctNoByIndex");
            }
            return acctNo;
        }
        public uint GetPINByIndex(int index) {
            uint pin;
            try {
                pin = storage[index].pin;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new DatabaseException("That Index does not exist", "GetPINByIndex");
            }
            catch (IndexOutOfRangeException) {
                throw new DatabaseException("That Index does not exist", "GetPINByIndex");
            }
            return pin;
        }
        public string GetFirstNameByIndex(int index) {
            string firstName;
            try {
                firstName = storage[index].firstName;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new DatabaseException("That Index does not exist", "GetFirstNameByIndex");
            }
            catch (IndexOutOfRangeException) {
                throw new DatabaseException("That Index Does Not exist", "GetFirstNameByIndex");
            }
            return firstName;
        }
        public string GetLastNameByIndex(int index) {
            string lastName;
            try {
                lastName = storage[index].lastName;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new DatabaseException("That Index does not exist", "GetLastNameByIndex");
            }
            catch (IndexOutOfRangeException) {
                throw new DatabaseException("That Index Does Not exist", "GetLastNameByIndex");
            }
            return lastName;
        }
        public int GetBalanceByIndex(int index) {
            int balance;
            try {
                balance = storage[index].balance;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new DatabaseException("That Index does not exist", "GetBalanceByIndex");
            }
            catch (IndexOutOfRangeException) {
                throw new DatabaseException("That Index Does Not exist", "GetBalanceByIndex");
            }
            return balance;
        }
        public Bitmap GetPictureByIndex(int index)
        {
            Bitmap picture;
            try
            {
                picture = storage[index].picture;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new DatabaseException("That Index does not exist", "GetPictureByIndex");
            }
            catch (IndexOutOfRangeException)
            {
                throw new DatabaseException("That Index Does Not exist", "GetPictureByIndex");
            }
            return picture;
        }
        public int GetNumRecords() {
            return storage.Count;
        }
    }
}
