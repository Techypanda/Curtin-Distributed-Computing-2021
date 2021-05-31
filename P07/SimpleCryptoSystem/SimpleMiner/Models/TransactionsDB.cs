using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleLibrary;
using System.Threading;
using SimplePayloads;
using SimpleMiner.Exceptions;
using Newtonsoft.Json;
using RestSharp;
using SimpleResponses;
using System.Security.Cryptography;

namespace SimpleMiner.Models {
    public class TransactionsDB {
        private static Queue<AddBlockRequest> _requests = new Queue<AddBlockRequest>();
        private static Semaphore _requestAlerter = new Semaphore(0, 4); // 4 threads going to be workers.
        static TransactionsDB() {
            for (int i = 0; i < 4; i++) {
                Thread t = new Thread(new ParameterizedThreadStart(SemaphoreWorker));
                t.Start(i);
            }
        }
        public void AddTransaction(AddBlockRequest req) {
            try {
                ValidateDetails(req);
                _requests.Enqueue(req);
                _requestAlerter.Release(1);
            } catch (TransactionException e) {
                throw e;
            }
        }
        private void ValidateDetails(AddBlockRequest req) {
            if (req.amount <= 0) {
                throw new TransactionException("The amount attached to transaction is invalid");
            }
            RestClient client = new RestClient("https://localhost:44362/");
            RestRequest request = new RestRequest($"api/Crypto/Balance/{req.fromWallet}");
            IRestResponse resp = client.Get(request);
            if (resp.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new TransactionException(resp.Content);
            }
            CryptoBalanceResponse decrypted = JsonConvert.DeserializeObject<CryptoBalanceResponse>(resp.Content);
            if (decrypted.currency - req.amount < 0) {
                throw new TransactionException("From Wallet does not have the sufficient funds to mine");
            }
        }

        private static void SemaphoreWorker(object num) {
            while (true) {
                Console.WriteLine($"Thread {num} - Waiting On A BlockRequest");
                _requestAlerter.WaitOne();
                AddBlockRequest blockRequest = _requests.Dequeue();
                RestClient client = new RestClient("https://localhost:44362/");
                RestRequest request = new RestRequest($"api/Crypto/State");
                IRestResponse resp = client.Get(request);
                CryptoStateResponse decrypted = JsonConvert.DeserializeObject<CryptoStateResponse>(resp.Content);
                Block lastBlock = decrypted.state.Last();
                Block block = new Block() { BlockID = lastBlock.BlockID + 1, Amount = blockRequest.amount, PreviousHash = lastBlock.Hash, WalletIDFrom = blockRequest.fromWallet, WalletIDTo = blockRequest.toWallet };
                GenerateHash(block, out uint offset, out string hash);
                block.BlockOffset = offset;
                block.Hash = hash;
                request = new RestRequest("api/Crypto/SubmitBlock");
                request.AddJsonBody(block);
                client.Post(request);
                Console.WriteLine($"Thread {num} - Finished BlockRequest");
            }
        }

        private static void GenerateHash(Block block, out uint offset, out string hash) {
            SHA256 sha256 = SHA256.Create();
            string bruteForcedHash = "";
            uint tempOffset = 0;
            while (!((bruteForcedHash.StartsWith("12345") /* && bruteForcedHash.EndsWith("54321") */))) {
                string temp = $"{block.WalletIDFrom}{block.WalletIDTo}{block.Amount}{tempOffset}{block.PreviousHash}";
                bruteForcedHash = BitConverter.ToUInt64(sha256.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes(temp)), 0).ToString();
                if (!((bruteForcedHash.StartsWith("12345") /* && bruteForcedHash.EndsWith("54321") */))) {
                    tempOffset += 5;
                }
            }
            offset = tempOffset;
            hash = bruteForcedHash;
        }
    }
}