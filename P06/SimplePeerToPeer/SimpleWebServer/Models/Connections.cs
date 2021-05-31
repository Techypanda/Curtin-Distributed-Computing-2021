using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleWebServer.Exceptions;
using SimpleWebServerResponses;
using System.ServiceModel;
using SimpleServerThread;
using System.Threading;
using System.Runtime.Remoting.Messaging;

namespace SimpleWebServer.Models {
    public class Connections {
        private readonly Dictionary<string, Connection> _connections = new Dictionary<string, Connection>();
        private readonly Dictionary<string, uint> _scores = new Dictionary<string, uint>();

        private delegate void Job();
        public Connections() {
            AsyncCallback jCallback = new AsyncCallback(this.connectionsCallback);
            Job job = new Job(this.validateConnections);
            job.BeginInvoke(this.connectionsCallback, null);
        }

        public void IncrementScore(string Username) {
            if (_scores.ContainsKey(Username)) {
                _scores[Username] = _scores[Username] + 1;
            }
        }

        public uint GetScore(string Username) {
            if (_scores.ContainsKey(Username)) {
                return _scores[Username];
            }
            return 0;
        }

        public void AddConnection(Connection con) {
            if (_connections.ContainsKey(con.Username)) {
                throw new ConnectionException("That username has already got a connection!");
            }
            _connections.Add(con.Username, con);
            _scores.Add(con.Username, 0);
        }
        public void RemoveConnection(string username) {
            if (!_connections.ContainsKey(username)) {
                throw new ConnectionException("That username has not got a connection!");
            }
            _connections.Remove(username);
            _scores.Remove(username);
        }
        public List<Connection> GetConnections() {
            return _connections.Values.ToList();
        }

        private void validateConnections() {
            GetConnections().ForEach((Connection con) => {
                try {
                    NetTcpBinding tcp = new NetTcpBinding();
                    string URL = $"net.tcp://{con.IPAddress}:{con.Port}/P2PServer";
                    using (ChannelFactory<P2PServerContract> conFactory = new ChannelFactory<P2PServerContract>(tcp, URL)) {
                        P2PServerContract conn = conFactory.CreateChannel();
                        conn.GetJobs(); // if this succeeds the connection is valid.
                    }
                } catch (Exception e) { // Connection failed, remove it its dead
                    try {
                        RemoveConnection(con.Username);
                    } catch (ConnectionException) {
                        // Not In Cons Anymore!!
                    }
                }
            });
            Thread.Sleep(10000); // Every 10 seconds run it again
        }
        private void connectionsCallback(IAsyncResult asyncResult) {
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            if (asyncObj.EndInvokeCalled == false) {
                Job clearconnectionThread = (Job)asyncObj.AsyncDelegate;
                clearconnectionThread.EndInvoke(asyncObj);
                AsyncCallback jCallback = new AsyncCallback(this.connectionsCallback);
                Job job = new Job(this.validateConnections);
                job.BeginInvoke(jCallback, null);
            }
        }
    }
}