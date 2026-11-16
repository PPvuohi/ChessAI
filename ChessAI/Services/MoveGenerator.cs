using ChessAI.Models;
using System.Collections.Generic;

namespace ChessAI.Services
{
    public class MoveGenerator
    {
        public List<Move> GenerateMoves(BoardState position)
        {
            List<Move> moves = new List<Move>();

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    char piece = position.Squares[row, col];

                    if (piece == '.')
                        continue;

                    bool isWhite = char.IsUpper(piece);

                    if (isWhite != position.WhiteToMove)
                        continue;

                    switch (char.ToUpper(piece))
                    {
                        case 'P':
                            GeneratePawnMoves(position, moves, row, col, isWhite);
                            break;

                        case 'N':
                            GenerateKnightMoves(position, moves, row, col, isWhite);
                            break;

                        case 'B':
                            GenerateSlidingMoves(
                                position, moves, row, col, isWhite,
                                new int[,]
                                {
                                    { -1, -1 },
                                    { -1, 1 },
                                    { 1, -1 },
                                    { 1, 1 }
                                });
                            break;

                        case 'R':
                            GenerateSlidingMoves(
                                position, moves, row, col, isWhite,
                                new int[,]
                                {
                                    { -1, 0 },
                                    { 1, 0 },
                                    { 0, -1 },
                                    { 0, 1 }
                                });
                            break;

                        case 'Q':
                            GenerateSlidingMoves(
                                position, moves, row, col, isWhite,
                                new int[,]
                                {
                                    { -1, 0 },
                                    { 1, 0 },
                                    { 0, -1 },
                                    { 0, 1 },
                                    { -1, -1 },
                                    { -1, 1 },
                                    { 1, -1 },
                                    { 1, 1 }
                                });
                            break;

                        case 'K':
                            GenerateKingMoves(position, moves, row, col, isWhite);
                            break;
                    }
                }
            }

            return moves;
        }

        private void GeneratePawnMoves(
            BoardState position,
            List<Move> moves,
            int row,
            int col,
            bool isWhite)
        {
            int direction = isWhite ? -1 : 1;
            int nextRow = row + direction;

            if (IsInside(nextRow, col) &&
                position.Squares[nextRow, col] == '.')
            {
                AddMove(position, moves, row, col, nextRow, col);
            }

            int leftCol = col - 1;
            int rightCol = col + 1;

            if (IsInside(nextRow, leftCol) &&
                IsOpponent(position.Squares[nextRow, leftCol], isWhite))
            {
                AddMove(position, moves, row, col, nextRow, leftCol);
            }

            if (IsInside(nextRow, rightCol) &&
                IsOpponent(position.Squares[nextRow, rightCol], isWhite))
            {
                AddMove(position, moves, row, col, nextRow, rightCol);
            }
        }

        private void GenerateKnightMoves(
            BoardState position,
            List<Move> moves,
            int row,
            int col,
            bool isWhite)
        {
            int[,] directions =
            {
                { -2, -1 },
                { -2, 1 },
                { -1, -2 },
                { -1, 2 },
                { 1, -2 },
                { 1, 2 },
                { 2, -1 },
                { 2, 1 }
            };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int newRow = row + directions[i, 0];
                int newCol = col + directions[i, 1];

                AddIfAllowed(
                    position,
                    moves,
                    row,
                    col,
                    newRow,
                    newCol,
                    isWhite);
            }
        }

        private void GenerateKingMoves(
            BoardState position,
            List<Move> moves,
            int row,
            int col,
            bool isWhite)
        {
            for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
            {
                for (int colOffset = -1; colOffset <= 1; colOffset++)
                {
                    if (rowOffset == 0 && colOffset == 0)
                        continue;

                    AddIfAllowed(
                        position,
                        moves,
                        row,
                        col,
                        row + rowOffset,
                        col + colOffset,
                        isWhite);
                }
            }
        }

        private void GenerateSlidingMoves(
            BoardState position,
            List<Move> moves,
            int row,
            int col,
            bool isWhite,
            int[,] directions)
        {
            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int rowDirection = directions[i, 0];
                int colDirection = directions[i, 1];

                int newRow = row + rowDirection;
                int newCol = col + colDirection;

                while (IsInside(newRow, newCol))
                {
                    char target = position.Squares[newRow, newCol];

                    if (target == '.')
                    {
                        AddMove(
                            position,
                            moves,
                            row,
                            col,
                            newRow,
                            newCol);
                    }
                    else
                    {
                        if (IsOpponent(target, isWhite))
                        {
                            AddMove(
                                position,
                                moves,
                                row,
                                col,
                                newRow,
                                newCol);
                        }

                        break;
                    }

                    newRow += rowDirection;
                    newCol += colDirection;
                }
            }
        }

        private void AddIfAllowed(
            BoardState position,
            List<Move> moves,
            int fromRow,
            int fromCol,
            int toRow,
            int toCol,
            bool isWhite)
        {
            if (!IsInside(toRow, toCol))
                return;

            char target = position.Squares[toRow, toCol];

            if (target == '.' || IsOpponent(target, isWhite))
            {
                AddMove(
                    position,
                    moves,
                    fromRow,
                    fromCol,
                    toRow,
                    toCol);
            }
        }

        private void AddMove(
            BoardState position,
            List<Move> moves,
            int fromRow,
            int fromCol,
            int toRow,
            int toCol)
        {
            moves.Add(new Move
            {
                FromRow = fromRow,
                FromCol = fromCol,
                ToRow = toRow,
                ToCol = toCol,
                Piece = position.Squares[fromRow, fromCol]
            });
        }

        private bool IsOpponent(char piece, bool isWhite)
        {
            if (piece == '.')
                return false;

            return char.IsUpper(piece) != isWhite;
        }

        private bool IsInside(int row, int col)
        {
            return row >= 0 && row < 8 &&
                   col >= 0 && col < 8;
        }
    }
}