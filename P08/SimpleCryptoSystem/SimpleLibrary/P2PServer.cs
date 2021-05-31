using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Threading;

namespace SimpleLibrary {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)] // Use .NET Synch
    public class P2PServer : P2PServerContract {
        public static readonly SimpleP2PChain _chain = new SimpleP2PChain();
        public static readonly Queue<Transaction> _transactions = new Queue<Transaction>();
        public static readonly Semaphore _transSemaphore = new Semaphore(0, int.MaxValue);

        public Block GetCurrentBlock() {
            return _chain.GetChain().Last();
        }

        public List<Block> GetCurrentBlockchain() {
            return _chain.GetChain();
        }

        public void RecieveTransaction(Transaction transaction) {
            try {
                ValidateDetails(transaction);
                _transactions.Enqueue(transaction);
                _transSemaphore.Release(1);
            } catch (P2PChainException e) {
                TransactionFault fault = new TransactionFault();
                fault.Operation = "transaction";
                fault.Reason = e.Message;
                throw new FaultException<TransactionFault>(fault, new FaultReason(e.Message));
            }
        }

        private void ValidateDetails(Transaction trans) {
            if (trans.Amount <= 0) {
                throw new P2PChainException("The amount attached to transaction is invalid");
            }
            if (_chain.CalculateWalletCoins(trans.FromWallet) - trans.Amount < 0) {
                throw new P2PChainException("From Wallet does not have the sufficient funds to mine");
            }
        }

    }
}
