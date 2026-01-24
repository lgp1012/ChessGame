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

        // Server address - bắt buộc phải nhập
        private string serverIP;
        private const int SERVER_PORT = 5000;

        public event Action<string> OnMessageReceived;
        public event Action<string> OnConnectionStatusChanged;
        public event Action<string> OnError;

        public bool IsConnected => isConnected;

        /// <summary>
        /// Constructor - IP server bắt buộc phải cung cấp
        /// </summary>
        public TcpClientConnection(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                throw new ArgumentException("Server IP không được để trống");
            
            serverIP = ip;
        }

        /// <summary>
        /// Connect to server and send player name
        /// </summary>
        public async Task ConnectAsync(string playerName)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.NoDelay = true;
                await tcpClient.ConnectAsync(serverIP, SERVER_PORT);
                
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
                    // Kiểm tra nếu server gửi tín hiệu đóng kết nối
                    if (line.Contains("[SERVER] Server is shutting down"))
                    {
                        OnMessageReceived?.Invoke(line);
                        Disconnect();
                        break;
                    }

                    // Nhận tin nhắn từ server
                    OnMessageReceived?.Invoke(line);
                }

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

        public void Disconnect()
        {
            if (!isConnected)
                return;

            isConnected = false;

            try
            {
                // Gửi tin nhắn ngắt kết nối đến server
                if (writer != null)
                {
                    try
                    {
                        writer.WriteLine("[CLIENT] Disconnecting");
                        writer.Flush();
                        Thread.Sleep(50);
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
