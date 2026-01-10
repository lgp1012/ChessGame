using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
                OnLogMessage?.Invoke("Server is shutting down...");

                // Send disconnection message to all clients first
                BroadcastCountdown("[SERVER] Server is shutting down. Connection will be closed.");

                // Give clients a moment to receive the message
                Thread.Sleep(200);

                // Disconnect all clients and trigger events
                List<ClientConnection> clientsToDisconnect = new List<ClientConnection>();
                lock (connectedClients)
                {
                    clientsToDisconnect.AddRange(connectedClients.Values);
                    connectedClients.Clear();
                }

                // Trigger disconnect event for each client
                foreach (var clientConn in clientsToDisconnect)
                {
                    try
                    {
                        clientConn.Writer?.Close();
                        clientConn.Client?.Close();
                    }
                    catch { }

                    // Trigger the disconnect event to update UI
                    OnClientDisconnected?.Invoke($"{clientConn.PlayerName}");
                }

                clientCount = 0;

                // Close listener
                try { tcpListener?.Stop(); } catch { }
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
                    if (isListening)
                    {
                        OnLogMessage?.Invoke($"Error accepting client: {ex.Message}");
                    }
                }
            }
        }

        private async void HandleClientAsync(TcpClient client, int clientId, string clientIp)
        {
            StreamWriter writer = null;
            StreamReader reader = null;
            string playerName = null;

            try
            {
                client.NoDelay = true;
                NetworkStream networkStream = client.GetStream();
                networkStream.ReadTimeout = 5000;

                writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true };
                reader = new StreamReader(networkStream, Encoding.UTF8);

                // Read player name from client
                playerName = await reader.ReadLineAsync();
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
                OnLogMessage?.Invoke($"{playerName} connected from {clientIp}");

                // Listen for messages from client
                string line;
                while (isListening && (line = await reader.ReadLineAsync()) != null)
                {
                    // Check if client is disconnecting
                    if (line.Contains("[CLIENT] Disconnecting"))
                    {
                        OnLogMessage?.Invoke($"{playerName} sent disconnect signal");
                        break;
                    }

                    OnLogMessage?.Invoke($"{playerName}: {line}");
                    BroadcastMessage($"{playerName}: {line}", clientId);
                }
            }
            catch (Exception ex)
            {
                OnLogMessage?.Invoke($"Error handling client: {ex.Message}");
            }
            finally
            {
                // Close all resources
                try { reader?.Dispose(); } catch { }
                try { writer?.Dispose(); } catch { }
                try { client?.Dispose(); } catch { }

                // Remove from client list and notify
                lock (connectedClients)
                {
                    if (connectedClients.TryGetValue(clientId, out var conn))
                    {
                        connectedClients.Remove(clientId);
                        playerName = conn.PlayerName;
                    }
                }

                if (!string.IsNullOrEmpty(playerName))
                {
                    OnClientDisconnected?.Invoke($"{playerName}");
                    OnLogMessage?.Invoke($"{playerName} disconnected. Total clients: {connectedClients.Count}");
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
        /// Send message to all connected clients
        /// </summary>
        public void BroadcastCountdown(string message)
        {
            lock (connectedClients)
            {
                foreach (var clientConn in connectedClients.Values.ToList())
                {
                    try
                    {
                        if (clientConn.Writer != null)
                        {
                            clientConn.Writer.WriteLine(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        OnLogMessage?.Invoke($"Error sending to {clientConn.PlayerName}: {ex.Message}");
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
                foreach (var kvp in connectedClients.ToList())
                {
                    if (kvp.Key != senderId)
                    {
                        try
                        {
                            if (kvp.Value.Writer != null)
                            {
                                kvp.Value.Writer.WriteLine(message);
                            }
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
