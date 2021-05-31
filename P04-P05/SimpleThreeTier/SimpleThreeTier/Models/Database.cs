using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BankDB;

namespace SimpleThreeTier.Models
{
    public static class Database
    {
        public static BankDB.BankDB bankDB = new BankDB.BankDB();
    }
}