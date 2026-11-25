namespace ChessAI.Models
{
    public class Move
    {
        public int FromRow { get; set; }
        public int FromCol { get; set; }

        public int ToRow { get; set; }
        public int ToCol { get; set; }

        public char Piece { get; set; }

        public bool Capture { get; set; } = false;
        public char? PromoteTo { get; set; } = null;

        public override string ToString()
        {
            string from = $"{(char)('A' + FromCol)}{8 - FromRow}";
            string to = $"{(char)('A' + ToCol)}{8 - ToRow}";

            return $"{from} -> {to}";
        }
    }
}