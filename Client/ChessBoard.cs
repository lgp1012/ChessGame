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
    }
}
