using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SimpleP2PServer.Models;
using SimpleLibrary;
using SimpleP2PServer.Exceptions;
using SimpleResponses;
using SimplePayloads;

namespace SimpleP2PServer.Controllers
{
    public class ConnectionsController : ApiController
    {
        private static readonly Connections _cons = new Connections();

        [Route("api/Connections/Register")]
        [HttpPost]
        public HttpResponseMessage Register(RegisterPayload payload) {
            try {
                Connection con = new Connection() { Username = payload.Username, IPAddress = payload.IPAddress, Port = payload.Port };
                _cons.AddConnection(con);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent($"Added A New Connection For User {payload.Username}, Don't forget to disconnect") };
            } catch (ConnectionException e) {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/Connections/Disconnect")]
        [HttpPost]
        public HttpResponseMessage Disconnect(DisconnectPayload payload) {
            try {
                _cons.RemoveConnection(payload.Username);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent($"Removed Connection For User {payload.Username}") };
            } catch (ConnectionException e) {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/Connections/FinishedJob")]
        [HttpPost]
        public HttpResponseMessage FinishedJob(FinishJobPayload payload) {
            try {
                _cons.IncrementScore(payload.Username);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent($"Incremented Score For User {payload.Username}") };
            } catch (ConnectionException e) {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [Route("api/Connections/Scoreboard")]
        [HttpGet]
        public GetScoreBoardResponse Scoreboard() {
            List<RankedConnection> finalRankedCons = new List<RankedConnection>();
            List<KeyValuePair<uint, Connection>> list = new List<KeyValuePair<uint, Connection>>();
            _cons.GetConnections().ForEach((Connection con) => {
                list.Add(new KeyValuePair<uint, Connection>(_cons.GetScore(con.Username), con));
            });
            list.Sort(delegate (KeyValuePair<uint, Connection> x, KeyValuePair<uint, Connection> y) {
                return y.Key.CompareTo(x.Key);
            });
            uint i = 0;
            list.ForEach((KeyValuePair<uint, Connection> rankedEntry) => {
                finalRankedCons.Add(new RankedConnection() { Con = rankedEntry.Value, Rank = i, Score = rankedEntry.Key });
                i += 1;
            });
            return new GetScoreBoardResponse() { Scoreboard = finalRankedCons };
        }

        [Route("api/Connections")]
        [HttpGet]
        public GetConnectionsResponse Connections() {
            return new GetConnectionsResponse() { connections = _cons.GetConnections() };
        }
    }
}
