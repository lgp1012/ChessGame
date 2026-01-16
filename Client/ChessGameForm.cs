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
        private bool isPausedByMe = false;
        private bool isBlockedByOpponent = false;
        private Button[,] cells;
        private int selectedRow = -1;
        private int selectedCol = -1;
        private bool shouldNotifyExit = true;
        private bool exitEventRaised = false;

        public event Action<string> OnGameMessage;
        public event Action OnGameExited;

        public ChessGameForm(PieceColor color, string name, string opponent)
        {
            InitializeComponent();
            chessBoard = new ChessBoard();
            playerColor = color;
            playerName = name;
            opponentName = opponent;
            isMyTurn = (color == PieceColor.White);

            this.Text = $"Chess Game - {playerName}";

            int boardSize = ChessBoard.BOARD_SIZE * squareSize;
            this.ClientSize = new Size(boardSize + 40, boardSize + 100);

            pauseOverlay.Location = new Point(20, 50);
            pauseOverlay.Size = new Size(boardSize, boardSize);

            InitializeChessBoard();
            UpdateTurnDisplay();
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
                    btn.BackColor = ((row + col) % 2 == 0) ? Color.Beige : Color.Brown;

                    int r = row, c = col;
                    btn.Click += (s, e) => CellClick(r, c);

                    cells[row, col] = btn;
                    this.Controls.Add(btn);
                }
            }

            UpdateBoardDisplay();
        }

        private void GetDisplayCoordinates(int boardRow, int boardCol, out int displayRow, out int displayCol)
        {
            if (playerColor == PieceColor.Black)
            {
                displayRow = 7 - boardRow;
                displayCol = 7 - boardCol;
            }
            else
            {
                displayRow = boardRow;
                displayCol = boardCol;
            }
        }

        private void GetBoardCoordinates(int displayRow, int displayCol, out int boardRow, out int boardCol)
        {
            if (playerColor == PieceColor.Black)
            {
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
                    GetDisplayCoordinates(boardRow, boardCol, out int displayRow, out int displayCol);

                    ChessPiece piece = chessBoard.GetPiece(boardRow, boardCol);
                    cells[displayRow, displayCol].Text = piece != null ? GetPieceSymbol(piece.Type, piece.Color) : "";
                    cells[displayRow, displayCol].ForeColor = piece != null && piece.Color == PieceColor.White ? Color.White : Color.Black;
                    cells[displayRow, displayCol].BackColor = ((displayRow + displayCol) % 2 == 0) ? Color.Beige : Color.Brown;
                }
            }
        }

        private string GetPieceSymbol(PieceType type, PieceColor color)
        {
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

            GetBoardCoordinates(displayRow, displayCol, out int row, out int col);

            if (selectedRow == -1)
            {
                ChessPiece piece = chessBoard.GetPiece(row, col);
                if (piece != null && piece.Color == playerColor)
                {
                    selectedRow = row;
                    selectedCol = col;
                    HighlightMoves(row, col);
                }
            }
            else
            {
                if (row == selectedRow && col == selectedCol)
                {
                    ClearSelection();
                }
                else if (chessBoard.IsValidMoveSafe(selectedRow, selectedCol, row, col))
                {
                    ExecuteMove(selectedRow, selectedCol, row, col);
                }
                else
                {
                    ClearSelection();
                }
            }
        }

        private void ExecuteMove(int r1, int c1, int r2, int c2)
        {
            chessBoard.MovePiece(r1, c1, r2, c2);
            ClearSelection();
            UpdateBoardDisplay();

            OnGameMessage?.Invoke($"[MOVE]{r1},{c1}->{r2},{c2}");

            PieceColor opponentColor = (playerColor == PieceColor.White) ? PieceColor.Black : PieceColor.White;
            if (chessBoard.IsCheckmate(opponentColor))
            {
                MessageBox.Show("Checkmate! Bạn đã thắng!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OnGameMessage?.Invoke("[CHECKMATE]");
                return;
            }

            isMyTurn = false;
            UpdateTurnDisplay();
        }

        private void HighlightMoves(int row, int col)
        {
            GetDisplayCoordinates(row, col, out int selectedDisplayRow, out int selectedDisplayCol);
            cells[selectedDisplayRow, selectedDisplayCol].BackColor = Color.Gold;

            for (int r = 0; r < ChessBoard.BOARD_SIZE; r++)
            {
                for (int c = 0; c < ChessBoard.BOARD_SIZE; c++)
                {
                    if (chessBoard.IsValidMoveSafe(row, col, r, c))
                    {
                        GetDisplayCoordinates(r, c, out int displayR, out int displayC);

                        ChessPiece target = chessBoard.GetPiece(r, c);
                        cells[displayR, displayC].BackColor = target != null ? Color.IndianRed : Color.LightGreen;
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

        public void HandleOpponentMove(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleOpponentMove(message)));
                return;
            }

            try
            {
                // Handle CHECKMATE message
                if (message == "[CHECKMATE]")
                {
                    MessageBox.Show("Đối thủ đã chiếu bí bạn!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Parse message: "[MOVE]r1,c1->r2,c2"
                if (!message.StartsWith("[MOVE]"))
                {
                    return;
                }

                string moveData = message.Substring(6);
                string[] parts = moveData.Split(new[] { "->" }, StringSplitOptions.None);
                if (parts.Length != 2)
                {
                    // Vẫn đổi lượt dù có lỗi format
                    isMyTurn = true;
                    UpdateTurnDisplay();
                    return;
                }

                string[] from = parts[0].Split(',');
                string[] to = parts[1].Split(',');

                if (from.Length != 2 || to.Length != 2)
                {
                    isMyTurn = true;
                    UpdateTurnDisplay();
                    return;
                }

                int r1 = int.Parse(from[0]);
                int c1 = int.Parse(from[1]);
                int r2 = int.Parse(to[0]);
                int c2 = int.Parse(to[1]);

                // Thực hiện nước đi của đối thủ (bỏ validation để tránh lỗi sync)
                chessBoard.MovePiece(r1, c1, r2, c2);
                UpdateBoardDisplay();

                // LUÔN đổi lượt sau khi nhận move
                isMyTurn = true;
                UpdateTurnDisplay();

                // Kiểm tra chiếu bí
                if (chessBoard.IsCheckmate(playerColor))
                {
                    MessageBox.Show("Checkmate! Bạn đã thua!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // Vẫn đổi lượt dù có exception
                isMyTurn = true;
                UpdateTurnDisplay();
                System.Diagnostics.Debug.WriteLine($"Error parsing move: {ex.Message}");
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (!isPausedByMe)
            {
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
                shouldNotifyExit = false;
                RaiseGameExitedOnce();
                this.Close();
            }
        }

        public void SetTurn(bool myTurn)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetTurn(myTurn)));
                return;
            }
            
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

            if (cells != null)
            {
                foreach (var btn in cells)
                {
                    if (btn != null) btn.Enabled = false;
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
            isBlockedByOpponent = false;
            pauseOverlay.Visible = false;

            if (cells != null)
            {
                foreach (var btn in cells)
                {
                    if (btn != null) btn.Enabled = true;
                }
            }

            UpdateTurnDisplay();
        }

        public void ShowOpponentExitMessage(string opponentExitName)
        {
            shouldNotifyExit = false;
            MessageBox.Show($"{opponentExitName} đã thoát ván đấu. Ván đấu kết thúc!", "Đối thủ thoát",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            RaiseGameExitedOnce();
            this.Close();
        }

        public void GameStoppedByServer()
        {
            if (InvokeRequired)
            {
                // Use Invoke instead of BeginInvoke for synchronous execution to prevent race conditions
                // and ensure form closes properly before ClientForm attempts to show itself
                Invoke(new Action(() => GameStoppedByServer()));
                return;
            }

            // Prevent sending EXIT message and double-raising events
            shouldNotifyExit = false;
            exitEventRaised = true; // Prevent OnFormClosing from raising event again
            
            MessageBox.Show("Server đã dừng trận đấu.\nBạn sẽ quay về form kết nối.", "Trận đấu kết thúc",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        public void DisableExitNotification()
        {
            shouldNotifyExit = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Chỉ gửi [EXIT] nếu được phép
            if (shouldNotifyExit)
            {
                OnGameMessage?.Invoke($"[EXIT]{playerName}");
            }

            RaiseGameExitedOnce();
        }

        private void RaiseGameExitedOnce()
        {
            if (exitEventRaised) return;
            exitEventRaised = true;
            OnGameExited?.Invoke();
        }
    }
}
