using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Cryptography;
using SimpleBlockchainServer.Models;
using SimpleLibrary;
using SimpleBlockchainServer.Exceptions;
using SimpleResponses;

namespace SimpleBlockchainServer.Controllers
{
    public class CryptoController : ApiController
    {
        private static Chain _chain = new Chain();

        [Route("api/Crypto/State")]
        [HttpGet]
        public CryptoStateResponse State() {
            return new CryptoStateResponse() { state = _chain.GetChain() };
        }

        [Route("api/Crypto/Balance/{walletID}")]
        [HttpGet]
        public CryptoBalanceResponse Balance(uint walletID) {
            return new CryptoBalanceResponse() { currency = _chain.CalculateWalletCoins(walletID) };
        }

        [Route("api/Crypto/SubmitBlock")]
        [HttpPost]
        public HttpResponseMessage SubmitBlock(Block payload) {
            try {
                _chain.AddBlock(payload);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Successfully Submitted Block.") };
            } catch (BlockChainException bExcept) {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(bExcept.Message) });
            }
        }
    }
}
