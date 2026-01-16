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
        public event Action OnMatchShouldReset;
        public event Action<string, string> OnClientPaused;  // playerName, timestamp
        public event Action<string, string> OnClientResumed; // playerName, timestamp
        public event Action<string, string> OnClientExited;  // playerName, timestamp
        public event Action OnMatchEnded;                    // raised when a player exits

        private class ClientConnection
        {
            public int Id { get; set; }
            public TcpClient Client { get; set; }
            public StreamWriter Writer { get; set; }
            public string PlayerName { get; set; }
            public string IpAddress { get; set; }
        }

        public void Start()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, PORT);
                tcpListener.Start();
                isListening = true;

                OnLogMessage?.Invoke($"Server started on port {PORT}");

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

                BroadcastCountdown("[SERVER] Server is shutting down. Connection will be closed.");
                Thread.Sleep(200);

                List<ClientConnection> clientsToDisconnect = new List<ClientConnection>();
                lock (connectedClients)
                {
                    clientsToDisconnect.AddRange(connectedClients.Values);
                    connectedClients.Clear();
                }

                foreach (var clientConn in clientsToDisconnect)
                {
                    try { clientConn.Writer?.Close(); clientConn.Client?.Close(); } catch { }
                    OnClientDisconnected?.Invoke($"{clientConn.PlayerName}");
                }

                clientCount = 0;
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
                    string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    HandleClientAsync(client, clientId, clientIp);
                }
                catch (ObjectDisposedException)
                {
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

                playerName = await reader.ReadLineAsync();
                if (playerName == null)
                {
                    client.Close();
                    return;
                }

                ClientConnection clientConn = new ClientConnection
                {
                    Id = clientId,
                    Client = client,
                    Writer = writer,
                    PlayerName = playerName,
                    IpAddress = clientIp
                };

                lock (connectedClients)
                {
                    connectedClients[clientId] = clientConn;
                }

                OnClientConnected?.Invoke($"{playerName} ({clientIp})");
                OnLogMessage?.Invoke($"{playerName} connected from {clientIp}");

                string line;
                while (isListening && (line = await reader.ReadLineAsync()) != null)
                {
                    if (line.Contains("[CLIENT] Disconnecting"))
                    {
                        OnLogMessage?.Invoke($"{playerName} sent disconnect signal");
                        break;
                    }

                    if (line.StartsWith("[MOVE]"))
                    {
                        BroadcastMessage(line, clientId);      // deliver move
                        BroadcastMessage("[TURN]", clientId);   // give turn to opponent
                        OnLogMessage?.Invoke($"{playerName} moved: {line}");
                        continue;
                    }

                    if (line.StartsWith("[PAUSE]"))
                    {
                        string timestamp = DateTime.Now.ToString("HH:mm:ss");
                        OnLogMessage?.Invoke($"[{timestamp}] {playerName} paused the game");
                        OnClientPaused?.Invoke(playerName, timestamp);
                        BroadcastMessage(line, clientId);
                        continue;
                    }

                    if (line.StartsWith("[RESUME]"))
                    {
                        string timestamp = DateTime.Now.ToString("HH:mm:ss");
                        OnLogMessage?.Invoke($"[{timestamp}] {playerName} resumed the game");
                        OnClientResumed?.Invoke(playerName, timestamp);
                        BroadcastMessage(line, clientId);
                        continue;
                    }

                    if (line.StartsWith("[EXIT]"))
                    {
                        string timestamp = DateTime.Now.ToString("HH:mm:ss");
                        OnLogMessage?.Invoke($"[{timestamp}] {playerName} exited the game");
                        OnClientExited?.Invoke(playerName, timestamp);
                        BroadcastMessage(line, clientId); // tell opponent to close
                        OnMatchEnded?.Invoke();           // update server UI
                        continue;
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
                try { reader?.Dispose(); } catch { }
                try { writer?.Dispose(); } catch { }
                try { client?.Dispose(); } catch { }

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

                    if (connectedClients.Count < 2)
                    {
                        OnMatchShouldReset?.Invoke();
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

        public void BroadcastCountdown(string message)
        {
            lock (connectedClients)
            {
                foreach (var clientConn in connectedClients.Values.ToList())
                {
                    try
                    {
                        clientConn.Writer?.WriteLine(message);
                    }
                    catch (Exception ex)
                    {
                        OnLogMessage?.Invoke($"Error sending to {clientConn.PlayerName}: {ex.Message}");
                    }
                }
            }
        }

        private void BroadcastMessage(string message, int senderId)
        {
            lock (connectedClients)
            {
                foreach (var kvp in connectedClients.ToList())
                {
                    if (kvp.Key == senderId) continue;

                    try { kvp.Value.Writer?.WriteLine(message); } catch { }
                }
            }
        }

        private void AssignColors()
        {
            lock (connectedClients)
            {
                if (connectedClients.Count == 2)
                {
                    var clients = connectedClients.Values.ToList();
                    clients[0].Writer.WriteLine("[OPPONENT]|" + clients[1].PlayerName + "|WHITE");
                    clients[1].Writer.WriteLine("[OPPONENT]|" + clients[0].PlayerName + "|BLACK");
                    OnLogMessage?.Invoke("Colors assigned: " + clients[0].PlayerName + " (WHITE) vs " + clients[1].PlayerName + " (BLACK)");
                }
            }
        }

        public void StartMatch()
        {
            lock (connectedClients)
            {
                if (connectedClients.Count == 2)
                {
                    var clients = connectedClients.Values.ToList();
                    try
                    {
                        clients[0].Writer.WriteLine("[OPPONENT]|" + clients[1].PlayerName + "|WHITE");
                        clients[1].Writer.WriteLine("[OPPONENT]|" + clients[0].PlayerName + "|BLACK");
                        OnLogMessage?.Invoke("Match started: " + clients[0].PlayerName + " (WHITE) vs " + clients[1].PlayerName + " (BLACK)");
                    }
                    catch (Exception ex)
                    {
                        OnLogMessage?.Invoke($"Error starting match: {ex.Message}");
                    }
                }
            }
        }
    }
}
