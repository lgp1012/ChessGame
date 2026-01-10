using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    internal class TcpServer
    {
        private TcpListener tcpListener;
        private bool isListening = false;
        private const int PORT = 5000;
        private int clientCount = 0;
        private Dictionary<int, ClientConnection> connectedClients = new Dictionary<int, ClientConnection>();

        public event Action<string> OnClientConnected;
        public event Action<string> OnClientDisconnected;
        public event Action<string> OnLogMessage;

        private class ClientConnection
        {
            public int Id { get; set; }
            public TcpClient Client { get; set; }
            public StreamWriter Writer { get; set; }
            public string PlayerName { get; set; }
        }

        public void Start()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, PORT);
                tcpListener.Start();
                isListening = true;

                OnLogMessage?.Invoke($"Server started on port {PORT}");

                // Accept clients asynchronously
                AcceptClientsAsync();
            }
            catch (Exception ex)
            {
                OnLogMessage?.Invoke($"Error starting server: {ex.Message}");
            }
        }

        public void Stop()
        {
            try
            {
                isListening = false;

                // Send disconnection message to all clients first
                BroadcastCountdown("[SERVER] Server is shutting down. Connection will be closed.");

                // Give clients a moment to receive the message
                System.Threading.Thread.Sleep(100);

                // Disconnect all clients
                lock (connectedClients)
                {
                    foreach (var clientConn in connectedClients.Values)
                    {
                        try
                        {
                            clientConn.Writer?.Close();
                            clientConn.Client?.Close();
                        }
                        catch { }
                    }
                    connectedClients.Clear();
                }
                clientCount = 0;

                tcpListener?.Stop();
                OnLogMessage?.Invoke("Server stopped");
            }
            catch (Exception ex)
            {
                OnLogMessage?.Invoke($"Error stopping server: {ex.Message}");
            }
        }

        private async void AcceptClientsAsync()
        {
            while (isListening)
            {
                try
                {
                    TcpClient client = await tcpListener.AcceptTcpClientAsync();
                    clientCount++;
                    int clientId = clientCount;

                    // Get client IP address
                    string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                    // Handle client communication asynchronously
                    HandleClientAsync(client, clientId, clientIp);
                }
                catch (ObjectDisposedException)
                {
                    // Server was stopped
                    break;
                }
                catch (Exception ex)
                {
                    OnLogMessage?.Invoke($"Error accepting client: {ex.Message}");
                }
            }
        }

        private async void HandleClientAsync(TcpClient client, int clientId, string clientIp)
        {
            StreamWriter writer = null;
            StreamReader reader = null;

            try
            {
                writer = new StreamWriter(client.GetStream(), Encoding.UTF8) { AutoFlush = true };
                reader = new StreamReader(client.GetStream(), Encoding.UTF8);

                // Read player name from client
                string playerName = await reader.ReadLineAsync();
                if (playerName == null)
                {
                    client.Close();
                    return;
                }

                // Store client connection
                ClientConnection clientConn = new ClientConnection
                {
                    Id = clientId,
                    Client = client,
                    Writer = writer,
                    PlayerName = playerName
                };

                lock (connectedClients)
                {
                    connectedClients[clientId] = clientConn;
                }

                OnClientConnected?.Invoke($"{playerName} ({clientIp})");

                // Listen for messages from client
                string line;
                while (isListening && (line = await reader.ReadLineAsync()) != null)
                {
                    OnLogMessage?.Invoke($"{playerName}: {line}");
                    // Broadcast message to other clients if needed
                    BroadcastMessage($"{playerName}: {line}", clientId);
                }
            }
            catch (Exception ex)
            {
                OnLogMessage?.Invoke($"Error handling client {clientId}: {ex.Message}");
            }
            finally
            {
                reader?.Close();
                writer?.Close();
                client?.Close();

                lock (connectedClients)
                {
                    if (connectedClients.TryGetValue(clientId, out var clientConn))
                    {
                        OnClientDisconnected?.Invoke($"{clientConn.PlayerName}");
                        connectedClients.Remove(clientId);
                    }
                }
            }
        }

        public int GetConnectedClientCount()
        {
            lock (connectedClients)
            {
                return connectedClients.Count;
            }
        }

        /// <summary>
        /// Send countdown message to all connected clients
        /// </summary>
        public void BroadcastCountdown(string message)
        {
            lock (connectedClients)
            {
                foreach (var clientConn in connectedClients.Values)
                {
                    try
                    {
                        clientConn.Writer?.WriteLine(message);
                    }
                    catch (Exception ex)
                    {
                        OnLogMessage?.Invoke($"Error sending countdown to {clientConn.PlayerName}: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Broadcast a message to all clients except the sender
        /// </summary>
        private void BroadcastMessage(string message, int senderId)
        {
            lock (connectedClients)
            {
                foreach (var kvp in connectedClients)
                {
                    if (kvp.Key != senderId)
                    {
                        try
                        {
                            kvp.Value.Writer?.WriteLine(message);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
