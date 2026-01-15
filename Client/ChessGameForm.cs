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
            UpdateTurnDisplay();
            
            int boardSize = ChessBoard.BOARD_SIZE * squareSize;
            this.ClientSize = new Size(boardSize + 40, boardSize + 250);
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawChessBoard(e.Graphics);
        }

        private void DrawChessBoard(Graphics g)
        {
            int startX = 20;
            int startY = 100;

            // Draw opponent pieces (top) - Black pieces if player is White, White pieces if player is Black
            DrawPlayerPieces(g, startX, startY - 80, (playerColor == PieceColor.White) ? PieceColor.Black : PieceColor.White, true);

            // Draw board squares
            for (int row = 0; row < ChessBoard.BOARD_SIZE; row++)
            {
                for (int col = 0; col < ChessBoard.BOARD_SIZE; col++)
                {
                    int x = startX + col * squareSize;
                    int y = startY + row * squareSize;

                    // Alternate colors - red and white
                    Color squareColor = ((row + col) % 2 == 0) ? Color.White : Color.Crimson;
                    g.FillRectangle(new SolidBrush(squareColor), x, y, squareSize, squareSize);
                    g.DrawRectangle(Pens.Black, x, y, squareSize, squareSize);

                    // Draw pieces
                    ChessPiece piece = chessBoard.GetPiece(row, col);
                    if (piece != null)
                    {
                        DrawPiece(g, piece, x + squareSize / 2, y + squareSize / 2);
                    }
                }
            }

            // Draw coordinates
            for (int i = 0; i < ChessBoard.BOARD_SIZE; i++)
            {
                // Column labels (A-H)
                g.DrawString(((char)('A' + i)).ToString(), this.Font, Brushes.Black, 
                    startX + i * squareSize + squareSize / 2 - 5, startY + ChessBoard.BOARD_SIZE * squareSize + 5);

                // Row labels (8-1)
                g.DrawString((ChessBoard.BOARD_SIZE - i).ToString(), this.Font, Brushes.Black,
                    startX - 20, startY + i * squareSize + squareSize / 2 - 5);
            }

            // Draw player pieces (bottom)
            DrawPlayerPieces(g, startX, startY + ChessBoard.BOARD_SIZE * squareSize + 30, playerColor, false);
        }

        private void DrawPlayerPieces(Graphics g, int startX, int startY, PieceColor color, bool isOpponent)
        {
            // Display player's remaining pieces or opponent's pieces
            string label = isOpponent ? $"{opponentName}'s Pieces" : "Your Pieces";
            g.DrawString(label, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, startX, startY - 20);
        }

        private void DrawPiece(Graphics g, ChessPiece piece, int x, int y)
        {
            string pieceSymbol = GetPieceSymbol(piece.Type);
            Color circleColor = (piece.Color == PieceColor.White) ? Color.LightGray : Color.Black;
            Color textColor = (piece.Color == PieceColor.White) ? Color.Black : Color.White;
            
            // Draw circle background
            g.FillEllipse(new SolidBrush(circleColor), x - 20, y - 20, 40, 40);
            g.DrawEllipse(Pens.Black, x - 20, y - 20, 40, 40);

            // Draw piece text
            using (Font font = new Font("Arial", 14, FontStyle.Bold))
            {
                SizeF size = g.MeasureString(pieceSymbol, font);
                g.DrawString(pieceSymbol, font, new SolidBrush(textColor),
                    x - size.Width / 2, y - size.Height / 2);
            }
        }

        private string GetPieceSymbol(PieceType type)
        {
            switch (type)
            {
                case PieceType.Pawn: return "♟";
                case PieceType.Rook: return "♜";
                case PieceType.Knight: return "♞";
                case PieceType.Bishop: return "♝";
                case PieceType.Queen: return "♛";
                case PieceType.King: return "♚";
                default: return "";
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Tiếp tục ván đấu?", "Pause", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                isPaused = true;
                OnGameMessage?.Invoke($"[PAUSE]{playerName}");
                UpdateTurnDisplay();
                this.Invalidate();
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
            this.Invalidate();
        }

        public void ShowOpponentPauseMessage(string opponentPausedName)
        {
            isPaused = true;
            MessageBox.Show($"{opponentPausedName} đã tạm dừng ván đấu. Vui lòng chờ!", "Đối thủ tạm dừng",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            UpdateTurnDisplay();
            this.Invalidate();
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
