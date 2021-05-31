using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDLL {
    public class Database {
        List<DatabaseStorage> storage;
        public Database() {
            DatabaseGenerator localGenerator = new DatabaseGenerator();
            storage = new List<DatabaseStorage>();
            for (int i = 0; i < 100; i++) {
                DatabaseStorage newStorage = new DatabaseStorage();
                localGenerator.GetNextAccount(
                    out newStorage.pin,
                    out newStorage.acctNo,
                    out newStorage.firstName,
                    out newStorage.lastName,
                    out newStorage.balance
                );
                storage.Add(newStorage);
            }
        }
        public uint GetAcctNoByIndex(int index) {
            uint acctNo;
            try {
                acctNo = storage[index].acctNo;
            } catch (IndexOutOfRangeException) {
                acctNo = 0;
            }
            return acctNo;
        }
        public uint GetPINByIndex(int index) {
            uint pin;
            try {
                pin = storage[index].pin;
            } catch (IndexOutOfRangeException) {
                pin = 0;
            }
            return pin;
        }
        public string GetFirstNameByIndex(int index) {
            string firstName;
            try {
                firstName = storage[index].firstName;
            } catch (IndexOutOfRangeException) {
                firstName = "Error Has Occured";
            }
            return firstName;
        }
        public string GetLastNameByIndex(int index) {
            string lastName;
            try {
                lastName = storage[index].lastName;
            } catch (IndexOutOfRangeException) {
                lastName = "Error Has Occured";
            }
            return lastName;
        }
        public int GetBalanceByIndex(int index) {
            int balance;
            try {
                balance = storage[index].balance;
            } catch (IndexOutOfRangeException) {
                balance = 0;
            }
            return balance;
        }
        public int GetNumRecords() {
            return storage.Count;
        }
    }
}
