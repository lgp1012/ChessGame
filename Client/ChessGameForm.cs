using System;
using System.Drawing;
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
        private bool isPausedByMe = false; // Track if I initiated the pause
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
            
            // Position and size the pause overlay to cover the chess board area
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
                    
                    int r = row, c = col; // Capture cho lambda
                    btn.Click += (s, e) => CellClick(r, c);
                    
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
                    
                    ChessPiece piece = chessBoard.GetPiece(boardRow, boardCol);
                    cells[displayRow, displayCol].Text = piece != null ? GetPieceSymbol(piece.Type, piece.Color) : "";
                    cells[displayRow, displayCol].ForeColor = piece != null && piece.Color == PieceColor.White ? Color.White : Color.Black;
                    
                    // Reset màu nền về màu bàn cờ (dựa trên tọa độ hiển thị)
                    cells[displayRow, displayCol].BackColor = ((displayRow + displayCol) % 2 == 0) ? Color.Beige : Color.Brown;
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
            if (isPaused || !isMyTurn)
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
                    
                    // Validate opponent's move before accepting it
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
                        MessageBox.Show("Checkmate! Bạn đã thua!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            if (message == "[CHECKMATE]")
            {
                MessageBox.Show("Đối thủ đã chiếu bí bạn!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (!isPausedByMe)
            {
                // Currently not paused by me, show pause dialog
                DialogResult result = MessageBox.Show("Bạn muốn tạm dừng ván đấu?", "Pause", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    isPaused = true;
                    isPausedByMe = true;
                    OnGameMessage?.Invoke($"[PAUSE]{playerName}");
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
                OnGameMessage?.Invoke($"[RESUME]{playerName}");
                btnPause.Text = "Pause";
                btnPause.BackColor = Color.Orange;
                UpdateTurnDisplay();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn chắc chắn muốn thoát?", "Exit Game",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                OnGameMessage?.Invoke($"[EXIT]{playerName}");
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
            pauseLabel.Text = $"{opponentPausedName} đã tạm dừng trận đấu";
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
            
            UpdateTurnDisplay();
        }

        public void HideOpponentPauseOverlay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HideOpponentPauseOverlay()));
                return;
            }

            isPaused = false;
            pauseOverlay.Visible = false;
            
            // Enable all cell buttons
            if (cells != null)
            {
                foreach (var btn in cells)
                {
                    if (btn != null)
                        btn.Enabled = true;
                }
            }
            
            UpdateTurnDisplay();
        }

        public void ShowOpponentExitMessage(string opponentExitName)
        {
            MessageBox.Show($"{opponentExitName} đã thoát ván đấu. Ván đấu kết thúc!", "Đối thủ thoát",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            OnGameExited?.Invoke();
            this.Close();
        }

        public void GameStoppedByServer()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => GameStoppedByServer()));
                return;
            }

            MessageBox.Show("Server đã dừng trận đấu. Bạn sẽ quay về form kết nối.", "Trận đấu kết thúc",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            OnGameExited?.Invoke();
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            OnGameExited?.Invoke();
        }
    }
}
