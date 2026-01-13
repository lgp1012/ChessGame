using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class TcpClientConnection
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;
        private bool isConnected = false;

        // Server address - cố định
        private const string SERVER_IP = "192.168.80.1";
        private const int SERVER_PORT = 5000;

        public event Action<string> OnMessageReceived;
        public event Action<string> OnConnectionStatusChanged;
        public event Action<string> OnError;

        public bool IsConnected => isConnected;

        public TcpClientConnection()
        {
        }

        /// <summary>
        /// Connect to server and send player name
        /// </summary>
        public async Task ConnectAsync(string playerName)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.NoDelay = true;  // Disable Nagle's algorithm for real-time communication
                await tcpClient.ConnectAsync(SERVER_IP, SERVER_PORT);
                
                stream = tcpClient.GetStream();
                stream.ReadTimeout = 5000;  // 5 second read timeout
                
                writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                isConnected = true;

                // Send player name to server
                await writer.WriteLineAsync(playerName);

                OnConnectionStatusChanged?.Invoke("Connected");

                // Start listening for messages from server
                ListenToServerAsync();
            }
            catch (Exception ex)
            {
                isConnected = false;
                OnError?.Invoke($"Connection failed: {ex.Message}");
                OnConnectionStatusChanged?.Invoke("Disconnected");
                Disconnect();
            }
        }

        /// <summary>
        /// Listen to incoming messages from server asynchronously
        /// </summary>
        private async void ListenToServerAsync()
        {
            try
            {
                reader = new StreamReader(stream, Encoding.UTF8);
                string line;
                
                while (isConnected && (line = await reader.ReadLineAsync()) != null)
                {
                    // Check if server is shutting down
                    if (line.Contains("[SERVER] Server is shutting down"))
                    {
                        OnMessageReceived?.Invoke(line);
                        Disconnect();
                        break;
                    }

                    // Trigger event for received message
                    OnMessageReceived?.Invoke(line);
                }

                // If loop exits naturally (server closed connection)
                if (isConnected)
                {
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                if (isConnected)
                {
                    OnError?.Invoke($"Error receiving message: {ex.Message}");
                }
                if (isConnected)
                {
                    Disconnect();
                }
            }
            finally
            {
                try { reader?.Dispose(); } catch { }
            }
        }

        /// <summary>
        /// Send message to server
        /// </summary>
        public void SendMessage(string message)
        {
            try
            {
                if (isConnected && writer != null)
                {
                    writer.WriteLine(message);
                }
                else
                {
                    OnError?.Invoke("Not connected to server");
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Error sending message: {ex.Message}");
                if (isConnected)
                {
                    Disconnect();
                }
            }
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public void Disconnect()
        {
            if (!isConnected)
                return;

            isConnected = false;  // Set to false IMMEDIATELY to prevent race conditions

            try
            {
                // Try to send disconnect signal
                if (writer != null)
                {
                    try
                    {
                        writer.WriteLine("[CLIENT] Disconnecting");
                        writer.Flush();
                        Thread.Sleep(50);  // Give server time to receive
                    }
                    catch { }
                }
            }
            finally
            {
                try { writer?.Dispose(); } catch { }
                try { reader?.Dispose(); } catch { }
                try { stream?.Dispose(); } catch { }
                try { tcpClient?.Dispose(); } catch { }

                OnConnectionStatusChanged?.Invoke("Disconnected");
            }
        }
    }
}
