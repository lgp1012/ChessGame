using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Client
{
    public partial class ChessGameForm : Form
    {
        private ChessBoard chessBoard;
        private PieceColor playerColor;
        private string playerName;
        private string opponentName;
        private bool isMyTurn;
        private int squareSize = 60;
        private bool isPaused = false;
        private bool isPausedByMe = false;
        private bool isBlockedByOpponent = false; 
        private bool gameExitedTriggered = false;
        private Button[,] cells;
        private int selectedRow = -1;
        private int selectedCol = -1;
        private UdpGameClient udpClient;

        public event Action<string> OnGameMessage;
        public event Action OnGameExited;

        public ChessGameForm(PieceColor color, string name, string opponent, UdpGameClient udpClient = null)
        {
            InitializeComponent();
            chessBoard = new ChessBoard();
            playerColor = color;
            playerName = name;
            opponentName = opponent;
            isMyTurn = (color == PieceColor.White);
            this.udpClient = udpClient;

            this.Text = $"Chess Game - {playerName}";
            
            int boardSize = ChessBoard.BOARD_SIZE * squareSize;
            this.ClientSize = new Size(boardSize + 40, boardSize + 100);

            // Vi trí và kích thước overlay pause
            pauseOverlay.Location = new Point(20, 50);
            pauseOverlay.Size = new Size(boardSize, boardSize);
            
            InitializeChessBoard();
            UpdateTurnDisplay();
            
            // Đăng ký sự kiện UDP nếu có
            if (udpClient != null)
            {
                udpClient.OnMoveReceived += UdpClient_OnMoveReceived;
                udpClient.OnGameMessage += UdpClient_OnGameMessage;
            }
        }

        private void InitializeChessBoard()
        {
            cells = new Button[ChessBoard.BOARD_SIZE, ChessBoard.BOARD_SIZE];
            int startX = 20;
            int startY = 50;

            for (int row = 0; row < ChessBoard.BOARD_SIZE; row++)
            {
                for (int col = 0; col < ChessBoard.BOARD_SIZE; col++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(squareSize, squareSize);
                    btn.Location = new Point(startX + col * squareSize, startY + row * squareSize);
                    btn.Font = new Font("Arial", 24);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.TabStop = false;
                    
                    // Màu bàn cờ: Beige/Brown
                    btn.BackColor = ((row + col) % 2 == 0) ? Color.Beige : Color.Brown;
                    
                    int r = row, c = col;
                    btn.Click += (s, e) => CellClick(r, c);
                    
                    // Thêm custom paint để vẽ quân cờ với outline cho quân trắng
                    btn.Paint += (s, e) => DrawChessPiece(e.Graphics, btn, r, c);
                    
                    cells[row, col] = btn;
                    this.Controls.Add(btn);
                }
            }
            
            UpdateBoardDisplay();
        }
        
        // Chuyển đổi tọa độ hiển thị dựa trên màu quân
        private void GetDisplayCoordinates(int boardRow, int boardCol, out int displayRow, out int displayCol)
        {
            if (playerColor == PieceColor.Black)
            {
                // Xoay bàn cờ 180 độ cho người chơi quân đen
                displayRow = 7 - boardRow;
                displayCol = 7 - boardCol;
            }
            else
            {
                displayRow = boardRow;
                displayCol = boardCol;
            }
        }
        
        // Chuyển đổi tọa độ từ hiển thị sang bàn cờ
        private void GetBoardCoordinates(int displayRow, int displayCol, out int boardRow, out int boardCol)
        {
            if (playerColor == PieceColor.Black)
            {
                // Xoay ngược lại
                boardRow = 7 - displayRow;
                boardCol = 7 - displayCol;
            }
            else
            {
                boardRow = displayRow;
                boardCol = displayCol;
            }
        }

        private void UpdateTurnDisplay()
        {
            if (isPaused)
            {
                lblTurn.Text = "Game Paused";
                lblTurn.ForeColor = Color.Red;
            }
            else if (isMyTurn)
            {
                lblTurn.Text = "Your Turn";
                lblTurn.ForeColor = Color.Green;
            }
            else
            {
                lblTurn.Text = $"{opponentName}'s Turn";
                lblTurn.ForeColor = Color.Blue;
            }
        }

        private void UpdateBoardDisplay()
        {
            for (int boardRow = 0; boardRow < ChessBoard.BOARD_SIZE; boardRow++)
            {
                for (int boardCol = 0; boardCol < ChessBoard.BOARD_SIZE; boardCol++)
                {
                    // Lấy tọa độ hiển thị dựa trên màu quân
                    GetDisplayCoordinates(boardRow, boardCol, out int displayRow, out int displayCol);
                    
                    // Reset text về rỗng vì sẽ vẽ custom
                    cells[displayRow, displayCol].Text = "";
                    
                    // Reset màu nền về màu bàn cờ (dựa trên tọa độ hiển thị)
                    cells[displayRow, displayCol].BackColor = ((displayRow + displayCol) % 2 == 0) ? Color.Beige : Color.Brown;
                    
                    // Trigger repaint để vẽ quân cờ
                    cells[displayRow, displayCol].Invalidate();
                }
            }
        }

        private void DrawChessPiece(Graphics g, Button btn, int displayRow, int displayCol)
        {
            // Chuyển đổi từ tọa độ hiển thị sang tọa độ bàn cờ
            GetBoardCoordinates(displayRow, displayCol, out int boardRow, out int boardCol);
            
            ChessPiece piece = chessBoard.GetPiece(boardRow, boardCol);
            if (piece == null) return;

            string symbol = GetPieceSymbol(piece.Type, piece.Color);
            if (string.IsNullOrEmpty(symbol)) return;

            // Cài đặt font và vị trí
            Font font = new Font("Arial", 28, FontStyle.Regular);
            SizeF textSize = g.MeasureString(symbol, font);
            float x = (btn.Width - textSize.Width) / 2;
            float y = (btn.Height - textSize.Height) / 2;

            // Vẽ quân cờ
            if (piece.Color == PieceColor.White)
            {
                // Quân trắng: Vẽ outline đen trước, sau đó fill trắng
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddString(symbol, font.FontFamily, (int)FontStyle.Regular, 
                        g.DpiY * font.Size / 72, new PointF(x, y), StringFormat.GenericDefault);
                    
                    // Vẽ outline đen (stroke)
                    using (Pen outlinePen = new Pen(Color.Black, 3))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.DrawPath(outlinePen, path);
                    }
                    
                    // Fill màu trắng
                    using (SolidBrush fillBrush = new SolidBrush(Color.White))
                    {
                        g.FillPath(fillBrush, path);
                    }
                }
            }
            else
            {
                // Quân đen: Vẽ solid đen
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.DrawString(symbol, font, brush, x, y);
                }
            }
        }

        private string GetPieceSymbol(PieceType type, PieceColor color)
        {
            // Unicode cho quân trắng và đen
            if (color == PieceColor.White)
            {
                switch (type)
                {
                    case PieceType.King: return "♔";
                    case PieceType.Queen: return "♕";
                    case PieceType.Rook: return "♖";
                    case PieceType.Bishop: return "♗";
                    case PieceType.Knight: return "♘";
                    case PieceType.Pawn: return "♙";
                    default: return "";
                }
            }
            else
            {
                switch (type)
                {
                    case PieceType.King: return "♚";
                    case PieceType.Queen: return "♛";
                    case PieceType.Rook: return "♜";
                    case PieceType.Bishop: return "♝";
                    case PieceType.Knight: return "♞";
                    case PieceType.Pawn: return "♟";
                    default: return "";
                }
            }
        }

        private void CellClick(int displayRow, int displayCol)
        {
            if (isPaused || !isMyTurn || isBlockedByOpponent)
                return;

            // Chuyển đổi từ tọa độ hiển thị sang tọa độ bàn cờ
            GetBoardCoordinates(displayRow, displayCol, out int row, out int col);

            // Nếu chưa chọn quân nào
            if (selectedRow == -1)
            {
                ChessPiece piece = chessBoard.GetPiece(row, col);
                if (piece != null && piece.Color == playerColor)
                {
                    // Chọn quân
                    selectedRow = row;
                    selectedCol = col;
                    HighlightMoves(row, col);
                }
            }
            else
            {
                // Đã chọn quân, thử di chuyển
                if (row == selectedRow && col == selectedCol)
                {
                    // Click lại quân đã chọn -> bỏ chọn
                    ClearSelection();
                }
                else if (chessBoard.IsValidMoveSafe(selectedRow, selectedCol, row, col))
                {
                    // Di chuyển hợp lệ
                    ExecuteMove(selectedRow, selectedCol, row, col);
                }
                else
                {
                    // Nước đi không hợp lệ, bỏ chọn
                    ClearSelection();
                }
            }
        }

        private void ExecuteMove(int r1, int c1, int r2, int c2)
        {
            // Thực hiện nước đi
            chessBoard.MovePiece(r1, c1, r2, c2);
            ClearSelection();
            UpdateBoardDisplay();
            
            // Gửi nước đi qua UDP
            if (udpClient != null)
            {
                udpClient.SendMove(r1, c1, r2, c2);
            }
            else
            {
                // Fallback qua TCP nếu không có UDP
                OnGameMessage?.Invoke($"[MOVE]{r1},{c1}->{r2},{c2}");
            }
            
            // Kiểm tra chiếu bí
            PieceColor opponentColor = (playerColor == PieceColor.White) ? PieceColor.Black : PieceColor.White;
            if (chessBoard.IsCheckmate(opponentColor))
            {
                MessageBox.Show("Checkmate! Bạn đã thắng!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (udpClient != null)
                {
                    udpClient.SendMessage("[CHECKMATE]");
                }
                OnGameMessage?.Invoke("[CHECKMATE]");
                return;
            }
            
            // Đổi lượt
            isMyTurn = false;
            UpdateTurnDisplay();
        }

        private void HighlightMoves(int row, int col)
        {
            // Highlight quân được chọn (chuyển đổi sang tọa độ hiển thị)
            GetDisplayCoordinates(row, col, out int selectedDisplayRow, out int selectedDisplayCol);
            cells[selectedDisplayRow, selectedDisplayCol].BackColor = Color.Gold;
            
            // Highlight các nước đi hợp lệ
            for (int r = 0; r < ChessBoard.BOARD_SIZE; r++)
            {
                for (int c = 0; c < ChessBoard.BOARD_SIZE; c++)
                {
                    if (chessBoard.IsValidMoveSafe(row, col, r, c))
                    {
                        GetDisplayCoordinates(r, c, out int displayR, out int displayC);
                        
                        ChessPiece target = chessBoard.GetPiece(r, c);
                        if (target != null)
                        {
                            // Ô có quân đối phương
                            cells[displayR, displayC].BackColor = Color.IndianRed;
                        }
                        else
                        {
                            // Ô trống
                            cells[displayR, displayC].BackColor = Color.LightGreen;
                        }
                    }
                }
            }
        }

        private void ClearSelection()
        {
            selectedRow = -1;
            selectedCol = -1;
            UpdateBoardDisplay();
        }

        private void UdpClient_OnMoveReceived(string moveData)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UdpClient_OnMoveReceived(moveData)));
                return;
            }

            try
            {
                // Parse move: "r1,c1->r2,c2"
                string[] parts = moveData.Split(new[] { "->" }, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    string[] from = parts[0].Split(',');
                    string[] to = parts[1].Split(',');
                    
                    int r1 = int.Parse(from[0]);
                    int c1 = int.Parse(from[1]);
                    int r2 = int.Parse(to[0]);
                    int c2 = int.Parse(to[1]);

                    // Xác thực nước đi đối thủ
                    PieceColor opponentColor = (playerColor == PieceColor.White) ? PieceColor.Black : PieceColor.White;
                    ChessPiece movingPiece = chessBoard.GetPiece(r1, c1);
                    
                    if (movingPiece == null || movingPiece.Color != opponentColor)
                    {
                        MessageBox.Show("Invalid move received from opponent (wrong piece)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    
                    if (!chessBoard.IsValidMoveSafe(r1, c1, r2, c2))
                    {
                        MessageBox.Show("Invalid move received from opponent (illegal move)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    
                    // Thực hiện nước đi của đối thủ
                    chessBoard.MovePiece(r1, c1, r2, c2);
                    UpdateBoardDisplay();
                    
                    // Đổi lượt
                    isMyTurn = true;
                    UpdateTurnDisplay();
                    
                    // Kiểm tra chiếu bí
                    if (chessBoard.IsCheckmate(playerColor))
                    {
                        MessageBox.Show("Checkmate! You lose!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing move: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UdpClient_OnGameMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UdpClient_OnGameMessage(message)));
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[ChessGameForm] Received UDP message: {message}");

            if (message == "[CHECKMATE]")
            {
                MessageBox.Show("Đối thủ đã chiếu bí bạn!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (message == "[STOPMATCH]")
            {
                System.Diagnostics.Debug.WriteLine("[ChessGameForm] Received STOPMATCH via UDP - calling GameStoppedByServer");
                GameStoppedByServer();
            }
            else if (message.StartsWith("[PAUSE]"))
            {
                string pausedPlayerName = message.Substring(7); // Lấy tên người pause
                System.Diagnostics.Debug.WriteLine($"[ChessGameForm] Received PAUSE from {pausedPlayerName} via UDP");
                ShowOpponentPauseMessage(pausedPlayerName);
            }
            else if (message.StartsWith("[RESUME]"))
            {
                string resumedPlayerName = message.Substring(8); // Lấy tên người resume
                System.Diagnostics.Debug.WriteLine($"[ChessGameForm] Received RESUME from {resumedPlayerName} via UDP");
                HideOpponentPauseOverlay();
            }
            else if (message.StartsWith("[EXIT]"))
            {
                string exitPlayerName = message.Substring(6); // Lấy tên người exit
                System.Diagnostics.Debug.WriteLine($"[ChessGameForm] Received EXIT from {exitPlayerName} via UDP");
                ShowOpponentExitMessage(exitPlayerName);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (!isPausedByMe)
            {
                // Hiển thị hộp thoại xác nhận pause 
                DialogResult result = MessageBox.Show("Would you like to pause this match?", "Pause", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    isPaused = true;
                    isPausedByMe = true;
                    
                    // Hiển thị overlay cho chính mình
                    ShowMyPauseOverlay();
                    
                    // Gửi message qua CẢ TCP VÀ UDP để đối thủ nhận được ngay
                    OnGameMessage?.Invoke($"[PAUSE]{playerName}");
                    
                    // Gửi qua UDP để đảm bảo nhận nhanh
                    if (udpClient != null)
                    {
                        udpClient.SendMessage($"[PAUSE]{playerName}");
                    }
                    
                    btnPause.Text = "Resume";
                    btnPause.BackColor = Color.Green;
                    UpdateTurnDisplay();
                }
            }
            else
            {
                // Currently paused by me, resume
                isPaused = false;
                isPausedByMe = false;
                
                // Ẩn overlay của chính mình
                HideMyPauseOverlay();
                
                // Gửi message qua CẢ TCP VÀ UDP để đối thủ nhận được ngay
                OnGameMessage?.Invoke($"[RESUME]{playerName}");
                
                // Gửi qua UDP để đảm bảo nhận nhanh
                if (udpClient != null)
                {
                    udpClient.SendMessage($"[RESUME]{playerName}");
                }
                
                btnPause.Text = "Pause";
                btnPause.BackColor = Color.Orange;
                UpdateTurnDisplay();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Game",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Gửi message qua CẢ TCP VÀ UDP để đối thủ nhận được ngay
                OnGameMessage?.Invoke($"[EXIT]{playerName}");
                
                // Gửi qua UDP để đảm bảo nhận nhanh
                if (udpClient != null)
                {
                    udpClient.SendMessage($"[EXIT]{playerName}");
                }
                
                OnGameExited?.Invoke();
                this.Close();
            }
        }

        public void SetTurn(bool myTurn)
        {
            isMyTurn = myTurn;
            UpdateTurnDisplay();
        }

        public void ShowOpponentPauseMessage(string opponentPausedName)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowOpponentPauseMessage(opponentPausedName)));
                return;
            }

            isPaused = true;
            isBlockedByOpponent = true;
            pauseLabel.Text = $"Match is paused by {opponentPausedName}";
            pauseOverlay.Visible = true;
            pauseOverlay.BringToFront();
            
            // Disable all cell buttons
            if (cells != null)
            {
                foreach (var btn in cells)
                {
                    if (btn != null)
                        btn.Enabled = false;
                }
            }
            
            // Disable nút Pause vì đối phương đã pause
            btnPause.Enabled = false;
            
            UpdateTurnDisplay();
        }

        private void ShowMyPauseOverlay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowMyPauseOverlay()));
                return;
            }

            pauseLabel.Text = $"Match is paused by {playerName}";
            pauseOverlay.Visible = true;
            pauseOverlay.BringToFront();
            
            // Disable toàn bộ bàn cờ
            if (cells != null)
            {
                foreach (var btn in cells)
                {
                    if (btn != null)
                        btn.Enabled = false;
                }
            }
        }

        private void HideMyPauseOverlay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HideMyPauseOverlay()));
                return;
            }

            pauseOverlay.Visible = false;
            
            // Enable bàn cờ
            if (cells != null)
            {
                foreach (var btn in cells)
                {
                    if (btn != null)
                        btn.Enabled = true;
                }
            }
        }

        public void HideOpponentPauseOverlay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HideOpponentPauseOverlay()));
                return;
            }

            isPaused = false;
            isBlockedByOpponent = false;
            pauseOverlay.Visible = false;
            
            if (cells != null)
            {
                foreach (var btn in cells)
                {
                    if (btn != null)
                        btn.Enabled = true;
                }
            }
            
            // Enable lại nút Pause vì đối phương đã resume
            btnPause.Enabled = true;
            
            UpdateTurnDisplay();
        }

        public void ShowOpponentExitMessage(string opponentExitName)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowOpponentExitMessage(opponentExitName)));
                return;
            }

            MessageBox.Show($"{opponentExitName} đã thoát ván đấu. Ván đấu kết thúc!", "Đối thủ thoát",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            OnGameExited?.Invoke();
            this.Close();
        }


        public void GameStoppedByServer()
        {
            System.Diagnostics.Debug.WriteLine("[ChessGameForm] GameStoppedByServer called");
            
            if (InvokeRequired)
            {
                System.Diagnostics.Debug.WriteLine("[ChessGameForm] InvokeRequired = true, invoking...");
                Invoke(new Action(() => GameStoppedByServer()));
                return;
            }

            System.Diagnostics.Debug.WriteLine("[ChessGameForm] Showing MessageBox and closing form");
            
            MessageBox.Show("Server đã dừng trận đấu. Bạn sẽ quay về form kết nối.", "Trận đấu kết thúc",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            if (!gameExitedTriggered)
            {
                System.Diagnostics.Debug.WriteLine("[ChessGameForm] Invoking OnGameExited event");
                gameExitedTriggered = true;
                OnGameExited?.Invoke();
            }
            
            System.Diagnostics.Debug.WriteLine("[ChessGameForm] Setting DialogResult and closing");
            this.DialogResult = DialogResult.Abort;
            this.Close();
            
            System.Diagnostics.Debug.WriteLine("[ChessGameForm] Form closed");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Chỉ thuc hiện OnGameExited một lần duy nhất
            if (!gameExitedTriggered)
            {
                System.Diagnostics.Debug.WriteLine("[ChessGameForm] OnFormClosing - triggering OnGameExited");
                gameExitedTriggered = true;
                OnGameExited?.Invoke();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[ChessGameForm] OnFormClosing - OnGameExited already triggered, skipping");
            }
        }
    }
}
