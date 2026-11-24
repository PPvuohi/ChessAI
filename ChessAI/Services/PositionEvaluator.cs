using ChessAI.Models;

namespace ChessAI.Services
{
    public class PositionEvaluator
    {
        public int Evaluate(BoardState position)
        {
            int score = 0;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    char piece = position.Squares[row, col];

                    score += GetPieceValue(piece);
                }
            }

            return score;
        }

        private int GetPieceValue(char piece)
        {
            return piece switch
            {
                'P' => 1,
                'N' => 3,
                'B' => 3,
                'R' => 5,
                'Q' => 9,
                'K' => 40,

                'p' => -1,
                'n' => -3,
                'b' => -3,
                'r' => -5,
                'q' => -9,
                'k' => -40,

                _ => 0
            };
        }
    }
}