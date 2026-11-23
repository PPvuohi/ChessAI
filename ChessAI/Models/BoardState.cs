namespace ChessAI.Models
{
    public class BoardState
    {
        public char[,] Squares { get; set; } = new char[8, 8];

        // true = valkean vuoro, false = mustan vuoro
        public bool WhiteToMove { get; set; } = true;

        public BoardState()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Squares[row, col] = '.';
                }
            }
        }

        public BoardState Clone()
        {
            BoardState copy = new BoardState();

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    copy.Squares[row, col] = Squares[row, col];
                }
            }

            copy.WhiteToMove = WhiteToMove;

            return copy;
        }

        public void ApplyMove(Move move)
        {
            char piece = Squares[move.FromRow, move.FromCol];

            // Sotilas korotetaan automaattisesti kuningattareksi,
            // kun se saavuttaa laudan viimeisen rivin.
            if (piece == 'P' && move.ToRow == 0)
            {
                piece = 'Q';
            }
            else if (piece == 'p' && move.ToRow == 7)
            {
                piece = 'q';
            }

            Squares[move.ToRow, move.ToCol] = piece;
            Squares[move.FromRow, move.FromCol] = '.';

            WhiteToMove = !WhiteToMove;
        }
    }
}