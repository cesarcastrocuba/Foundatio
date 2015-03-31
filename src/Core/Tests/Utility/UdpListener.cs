using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Foundatio.Tests.Utility {
    public class UdpListener : IDisposable {
        private readonly List<string> _receivedMessages = new List<string>();
        private UdpClient _listener;
        private IPEndPoint _groupEndPoint;

        public UdpListener(string serverName, int port) {
            _listener = new UdpClient(new IPEndPoint(IPAddress.Parse(serverName), port)) { Client = { ReceiveTimeout = 2000 } };
            _groupEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }

        public List<string> GetMessages() {
            var result = new List<string>(_receivedMessages);
            _receivedMessages.Clear();

            return result;
        }

        public void StartListening(object expectedMessageCount = null) {
            if (expectedMessageCount == null)
                expectedMessageCount = 1;

            for (int index = 0; index < (int)expectedMessageCount; index++) {
                try {
                    byte[] data = _listener.Receive(ref _groupEndPoint);
                    _receivedMessages.Add(Encoding.ASCII.GetString(data, 0, data.Length));
                } catch (SocketException ex) {
                    // If we timeout, stop listening.
                    if (ex.ErrorCode == 10060)
                        continue;

                    throw;
                }
            }
        }

        public void Dispose() {
            if (_listener == null)
                return;

            _listener.Close();
            _listener = null;
        }
    }
}