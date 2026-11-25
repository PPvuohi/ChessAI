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

                    // Nappulan materiaaliarvo
                    score += GetPieceValue(piece) * 100;

                    // Nappulan sijainnista saatava lisäarvo
                    score += GetPositionBonus(piece, row, col);
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

        private int GetPositionBonus(char piece, int row, int col)
        {
            if (piece == '.')
                return 0;

            bool isWhite = char.IsUpper(piece);
            int sign = isWhite ? 1 : -1;

            switch (char.ToUpperInvariant(piece))
            {
                case 'P':
                    // Sotilas saa lisäpisteitä etenemisestä.
                    int progress = isWhite ? 6 - row : row - 1;
                    return sign * progress * 5;

                case 'N':
                    // Ratsu hyötyy keskeisestä sijainnista.
                    bool nearCenter =
                        row >= 2 && row <= 5 &&
                        col >= 2 && col <= 5;

                    return nearCenter ? sign * 10 : 0;

                default:
                    return 0;
            }
        }
    }
}
