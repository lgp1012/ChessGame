using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
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

        private const string SERVER_IP = "192.168.100.2";  // Change to server IP if on different machine
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
                await tcpClient.ConnectAsync(SERVER_IP, SERVER_PORT);
                stream = tcpClient.GetStream();
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
            }
            catch (Exception ex)
            {
                if (isConnected)
                {
                    OnError?.Invoke($"Error receiving message: {ex.Message}");
                }
            }
            finally
            {
                reader?.Dispose();
                if (isConnected)
                {
                    Disconnect();
                }
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
            }
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public void Disconnect()
        {
            try
            {
                isConnected = false;
                writer?.Close();
                reader?.Close();
                stream?.Close();
                tcpClient?.Close();
                OnConnectionStatusChanged?.Invoke("Disconnected");
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Error disconnecting: {ex.Message}");
            }
        }
    }
}
