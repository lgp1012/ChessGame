using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class UdpGameClient
    {
        private UdpClient udpClient;
        private IPEndPoint opponentEndPoint;
        private bool isRunning = false;
        private int localPort;

        public event Action<string> OnMoveReceived;
        public event Action<string> OnGameMessage;

        // Khởi tạo UDP client với port động
        public void Start(int requestedPort)
        {
            try
            {
                udpClient = new UdpClient(requestedPort);
                // Lấy port thực tế được gán (nếu dùng 0)
                this.localPort = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
                isRunning = true;
                
                // Bắt đầu nhận dữ liệu
                ReceiveAsync();
            }
            catch (Exception ex)
            {
                OnGameMessage?.Invoke($"[UDP] Error starting: {ex.Message}");
            }
        }

        // Kết nối đến đối thủ
        public void ConnectToOpponent(string ip, int port)
        {
            try
            {
                opponentEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                OnGameMessage?.Invoke($"[UDP] Connected to opponent at {ip}:{port}");
            }
            catch (Exception ex)
            {
                OnGameMessage?.Invoke($"[UDP] Error connecting to opponent: {ex.Message}");
            }
        }

        // Gửi nước đi: format "r1,c1->r2,c2"
        public void SendMove(int r1, int c1, int r2, int c2)
        {
            try
            {
                if (opponentEndPoint == null)
                {
                    OnGameMessage?.Invoke("[UDP] Opponent not connected");
                    return;
                }

                string moveData = $"{r1},{c1}->{r2},{c2}";
                byte[] data = Encoding.UTF8.GetBytes(moveData);
                udpClient.Send(data, data.Length, opponentEndPoint);
            }
            catch (Exception ex)
            {
                OnGameMessage?.Invoke($"[UDP] Error sending move: {ex.Message}");
            }
        }

        // Nhận nước đi từ đối thủ
        private async void ReceiveAsync()
        {
            while (isRunning)
            {
                try
                {
                    UdpReceiveResult result = await udpClient.ReceiveAsync();
                    string message = Encoding.UTF8.GetString(result.Buffer);
                    
                    // Kiểm tra format nước đi
                    if (message.Contains("->"))
                    {
                        OnMoveReceived?.Invoke(message);
                    }
                    else if (message.StartsWith("["))
                    {
                        // Game message (CHECKMATE, PAUSE, etc.)
                        OnGameMessage?.Invoke(message);
                    }
                }
                catch (ObjectDisposedException)
                {
                    // UDP client đã đóng
                    break;
                }
                catch (Exception ex)
                {
                    if (isRunning)
                    {
                        OnGameMessage?.Invoke($"[UDP] Error receiving: {ex.Message}");
                    }
                }
            }
        }

        // Gửi message (CHECKMATE, PAUSE, EXIT...)
        public void SendMessage(string message)
        {
            try
            {
                if (opponentEndPoint == null)
                {
                    OnGameMessage?.Invoke("[UDP] Opponent not connected");
                    return;
                }

                byte[] data = Encoding.UTF8.GetBytes(message);
                udpClient.Send(data, data.Length, opponentEndPoint);
            }
            catch (Exception ex)
            {
                OnGameMessage?.Invoke($"[UDP] Error sending message: {ex.Message}");
            }
        }

        // Dừng UDP client
        public void Stop()
        {
            isRunning = false;
            try
            {
                udpClient?.Close();
                udpClient?.Dispose();
            }
            catch { }
        }

        public int GetLocalPort()
        {
            return localPort;
        }
    }
}
