using System;
using System.Collections.Generic;

namespace Client
{
    public enum PieceType
    {
        None = 0,
        Pawn = 1,
        Rook = 2,
        Knight = 3,
        Bishop = 4,
        Queen = 5,
        King = 6
    }

    public enum PieceColor
    {
        White,
        Black
    }

    public class ChessPiece
    {
        public PieceType Type { get; set; }
        public PieceColor Color { get; set; }

        public ChessPiece(PieceType type, PieceColor color)
        {
            Type = type;
            Color = color;
        }
    }

    public class ChessBoard
    {
        private ChessPiece[,] board;
        public const int BOARD_SIZE = 8;

        public ChessBoard()
        {
            board = new ChessPiece[BOARD_SIZE, BOARD_SIZE];
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            // Initialize empty board
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    board[row, col] = null;
                }
            }

            // Set up white pieces (bottom)
            board[7, 0] = new ChessPiece(PieceType.Rook, PieceColor.White);
            board[7, 1] = new ChessPiece(PieceType.Knight, PieceColor.White);
            board[7, 2] = new ChessPiece(PieceType.Bishop, PieceColor.White);
            board[7, 3] = new ChessPiece(PieceType.Queen, PieceColor.White);
            board[7, 4] = new ChessPiece(PieceType.King, PieceColor.White);
            board[7, 5] = new ChessPiece(PieceType.Bishop, PieceColor.White);
            board[7, 6] = new ChessPiece(PieceType.Knight, PieceColor.White);
            board[7, 7] = new ChessPiece(PieceType.Rook, PieceColor.White);

            // White pawns
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                board[6, col] = new ChessPiece(PieceType.Pawn, PieceColor.White);
            }

            // Set up black pieces (top)
            board[0, 0] = new ChessPiece(PieceType.Rook, PieceColor.Black);
            board[0, 1] = new ChessPiece(PieceType.Knight, PieceColor.Black);
            board[0, 2] = new ChessPiece(PieceType.Bishop, PieceColor.Black);
            board[0, 3] = new ChessPiece(PieceType.Queen, PieceColor.Black);
            board[0, 4] = new ChessPiece(PieceType.King, PieceColor.Black);
            board[0, 5] = new ChessPiece(PieceType.Bishop, PieceColor.Black);
            board[0, 6] = new ChessPiece(PieceType.Knight, PieceColor.Black);
            board[0, 7] = new ChessPiece(PieceType.Rook, PieceColor.Black);

            // Black pawns
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                board[1, col] = new ChessPiece(PieceType.Pawn, PieceColor.Black);
            }
        }

        public ChessPiece GetPiece(int row, int col)
        {
            if (row < 0 || row >= BOARD_SIZE || col < 0 || col >= BOARD_SIZE)
                return null;
            return board[row, col];
        }

        public void SetPiece(int row, int col, ChessPiece piece)
        {
            if (row >= 0 && row < BOARD_SIZE && col >= 0 && col < BOARD_SIZE)
            {
                board[row, col] = piece;
            }
        }

        public void MovePiece(int fromRow, int fromCol, int toRow, int toCol)
        {
            if (fromRow < 0 || fromRow >= BOARD_SIZE || fromCol < 0 || fromCol >= BOARD_SIZE ||
                toRow < 0 || toRow >= BOARD_SIZE || toCol < 0 || toCol >= BOARD_SIZE)
                return;

            ChessPiece piece = board[fromRow, fromCol];
            board[toRow, toCol] = piece;
            board[fromRow, fromCol] = null;
        }

        // Kiểm tra có trong bàn cờ không
        public bool InBoard(int r, int c)
        {
            return r >= 0 && r < BOARD_SIZE && c >= 0 && c < BOARD_SIZE;
        }

        // Kiểm tra đường đi không bị chặn
        public bool ClearPath(int r1, int c1, int r2, int c2)
        {
            int rowDir = Math.Sign(r2 - r1);
            int colDir = Math.Sign(c2 - c1);
            
            int currentRow = r1 + rowDir;
            int currentCol = c1 + colDir;
            
            while (currentRow != r2 || currentCol != c2)
            {
                if (board[currentRow, currentCol] != null)
                    return false;
                    
                currentRow += rowDir;
                currentCol += colDir;
            }
            
            return true;
        }

        // Kiểm tra nước đi của Tốt (Pawn)
        bool PawnMove(ChessPiece p, int r1, int c1, int r2, int c2)
        {
            int direction = (p.Color == PieceColor.White) ? -1 : 1;
            int startRow = (p.Color == PieceColor.White) ? 6 : 1;
            
            // Di chuyển thẳng 1 ô
            if (c1 == c2 && r2 == r1 + direction && board[r2, c2] == null)
                return true;
            
            // Di chuyển thẳng 2 ô từ vị trí ban đầu
            if (c1 == c2 && r1 == startRow && r2 == r1 + 2 * direction && 
                board[r2, c2] == null && board[r1 + direction, c1] == null)
                return true;
            
            // Ăn chéo
            if (Math.Abs(c2 - c1) == 1 && r2 == r1 + direction && 
                board[r2, c2] != null && board[r2, c2].Color != p.Color)
                return true;
            
            return false;
        }

        // Kiểm tra nước đi cơ bản theo từng loại quân
        bool BasicMove(ChessPiece p, int r1, int c1, int r2, int c2)
        {
            switch (p.Type)
            {
                case PieceType.Pawn:
                    return PawnMove(p, r1, c1, r2, c2);
                
                case PieceType.Rook:
                    // Xe đi ngang hoặc dọc
                    return (r1 == r2 || c1 == c2) && ClearPath(r1, c1, r2, c2);
                
                case PieceType.Knight:
                    // Mã đi hình chữ L
                    int dr = Math.Abs(r2 - r1);
                    int dc = Math.Abs(c2 - c1);
                    return (dr == 2 && dc == 1) || (dr == 1 && dc == 2);
                
                case PieceType.Bishop:
                    // Tượng đi chéo
                    return Math.Abs(r2 - r1) == Math.Abs(c2 - c1) && ClearPath(r1, c1, r2, c2);
                
                case PieceType.Queen:
                    // Hậu = Xe + Tượng
                    return ((r1 == r2 || c1 == c2) || (Math.Abs(r2 - r1) == Math.Abs(c2 - c1))) && 
                           ClearPath(r1, c1, r2, c2);
                
                case PieceType.King:
                    // Vua đi 1 ô theo mọi hướng
                    return Math.Abs(r2 - r1) <= 1 && Math.Abs(c2 - c1) <= 1;
                
                default:
                    return false;
            }
        }

        // Kiểm tra nước đi hợp lệ
        public bool IsValidMove(int r1, int c1, int r2, int c2)
        {
            // Kiểm tra tọa độ hợp lệ
            if (!InBoard(r1, c1) || !InBoard(r2, c2))
                return false;
            
            // Không thể di chuyển đến cùng vị trí
            if (r1 == r2 && c1 == c2)
                return false;
            
            ChessPiece piece = board[r1, c1];
            if (piece == null)
                return false;
            
            ChessPiece target = board[r2, c2];
            
            // Không thể ăn quân cùng màu
            if (target != null && target.Color == piece.Color)
                return false;
            
            // Kiểm tra nước đi cơ bản theo luật quân cờ
            return BasicMove(piece, r1, c1, r2, c2);
        }

        // Tìm vị trí vua
        private Tuple<int, int> FindKing(PieceColor color)
        {
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    ChessPiece piece = board[r, c];
                    if (piece != null && piece.Type == PieceType.King && piece.Color == color)
                    {
                        return new Tuple<int, int>(r, c);
                    }
                }
            }
            return null;
        }

        // Kiểm tra vua có đang bị chiếu không
        public bool IsInCheck(PieceColor color)
        {
            var kingPos = FindKing(color);
            if (kingPos == null)
                return false;
            
            int kingRow = kingPos.Item1;
            int kingCol = kingPos.Item2;
            
            // Kiểm tra tất cả quân đối phương có thể tấn công vua không
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    ChessPiece piece = board[r, c];
                    if (piece != null && piece.Color != color)
                    {
                        if (BasicMove(piece, r, c, kingRow, kingCol))
                            return true;
                    }
                }
            }
            
            return false;
        }

        // Kiểm tra nước đi có để vua bị chiếu không
        bool MoveLeavesKingInCheck(PieceColor color, int r1, int c1, int r2, int c2)
        {
            // Lưu trạng thái hiện tại
            ChessPiece originalPiece = board[r2, c2];
            ChessPiece movingPiece = board[r1, c1];
            
            // Thực hiện nước đi tạm thời
            board[r2, c2] = movingPiece;
            board[r1, c1] = null;
            
            // Kiểm tra chiếu
            bool inCheck = IsInCheck(color);
            
            // Khôi phục trạng thái
            board[r1, c1] = movingPiece;
            board[r2, c2] = originalPiece;
            
            return inCheck;
        }

        // Kiểm tra nước đi an toàn (không tự chiếu vua)
        public bool IsValidMoveSafe(int r1, int c1, int r2, int c2)
        {
            if (!IsValidMove(r1, c1, r2, c2))
                return false;
            
            ChessPiece piece = board[r1, c1];
            if (piece == null)
                return false;
            
            return !MoveLeavesKingInCheck(piece.Color, r1, c1, r2, c2);
        }

        // Kiểm tra chiếu bí
        public bool IsCheckmate(PieceColor color)
        {
            // Phải đang bị chiếu
            if (!IsInCheck(color))
                return false;
            
            // Kiểm tra xem có nước đi nào thoát chiếu không
            for (int r1 = 0; r1 < BOARD_SIZE; r1++)
            {
                for (int c1 = 0; c1 < BOARD_SIZE; c1++)
                {
                    ChessPiece piece = board[r1, c1];
                    if (piece != null && piece.Color == color)
                    {
                        // Thử tất cả nước đi có thể
                        for (int r2 = 0; r2 < BOARD_SIZE; r2++)
                        {
                            for (int c2 = 0; c2 < BOARD_SIZE; c2++)
                            {
                                if (IsValidMoveSafe(r1, c1, r2, c2))
                                {
                                    // Có ít nhất 1 nước đi hợp lệ -> không phải chiếu bí
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            
            // Không có nước đi nào -> chiếu bí
            return true;
        }
    }
}
