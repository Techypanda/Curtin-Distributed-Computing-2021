using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleBlockchainServer.Exceptions;
using System.Security.Cryptography;
using SimpleLibrary;
namespace SimpleBlockchainServer.Models {
    public class Chain {
        private static List<Block> _blockchain = new List<Block>();

        public List<Block> GetChain() {
            return _blockchain;
        }

        static Chain() {
            GenerateGenesisBlock();
        }

        public void AddBlock(Block block) {
            if (CheckHashValid(block)) {
                _blockchain.ForEach((Block blockInDB) => {
                    if (blockInDB.BlockID >= block.BlockID) {
                        throw new BlockChainException("The Block's ID is not valid");
                    }
                });
                if (CalculateWalletCoins(block.WalletIDFrom) >= block.Amount) {
                    if (block.Amount <= 0) {
                        throw new BlockChainException("The Block's Amount Cannot Be Less Than Or Equal To 0");
                    }
                    if (block.BlockOffset % 5 != 0) {
                        throw new BlockChainException("The Block's Offset Is Incorrect");
                    }
                    if (!block.PreviousHash.Equals(_blockchain.Last().Hash)) {
                        throw new BlockChainException("The Previous Hash Is Invalid");
                    }
                    _blockchain.Add(block);
                } else {
                    throw new BlockChainException("The Block's From Wallet Does Not Have Sufficient Funds.");
                }
            } else {
                throw new BlockChainException("The Block's Hash Is Not Valid");
            }
        }

        public float CalculateWalletCoins(uint walletID) {
            float userCurrency = 0; // Every User Starts With $0
            _blockchain.ForEach((Block block) => {
                userCurrency = handleTransaction(walletID, userCurrency, block);
            });
            if (walletID == 0) {
                return float.MaxValue; // Wallet 0 is the server, it has infinite $$$
            }
            return userCurrency;
        }

        private static void GenerateGenesisBlock() {
            SHA256 sha256 = SHA256.Create();
            string bruteForcedHash = "";
            uint offset = 1111573595; // as genesis is always first we know this is the exact number that it will hash correctly.
            float amount = 0;
            while ( !((bruteForcedHash.StartsWith("12345") && bruteForcedHash.EndsWith("54321"))) ) { 
                string currentGenesis = $"{amount}{0}{0}{offset}"; // no previous hash!
                bruteForcedHash = BitConverter.ToUInt64(sha256.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes(currentGenesis)), 0).ToString();
                if (!((bruteForcedHash.StartsWith("12345") && bruteForcedHash.EndsWith("54321")))) {
                    offset += 5;
                }
            }
            Block genesisBlock = new Block() { Amount = amount, BlockID = 0, BlockOffset = offset, Hash = bruteForcedHash, PreviousHash = "", WalletIDFrom = 0, WalletIDTo = 0 };
            _blockchain.Add(genesisBlock);
        }

        private float handleTransaction(uint walletID, float currency, Block block) {
            if (block.WalletIDFrom == walletID) { // Sending Money
                currency -= block.Amount;
            }
            if (block.WalletIDTo == walletID) { // Recieving Money
                currency += block.Amount;
            }
            return currency;
        }

        private bool CheckHashValid(Block block) {
            if (block.Hash.StartsWith("12345") /* && block.Hash.EndsWith("54321") */) {
                string computedHash = ComputeHash(block);
                if (computedHash.Equals(block.Hash)) {
                    return true;
                }
            }
            return false;
        }
        
        private string ComputeHash(Block block) {
            SHA256 sha256 = SHA256.Create();
            string blockString = $"{block.WalletIDFrom}{block.WalletIDTo}{block.Amount}{block.BlockOffset}{block.PreviousHash}";
            return BitConverter.ToUInt64(sha256.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes(blockString)), 0).ToString();
        }
    }
}