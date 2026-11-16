namespace ChessAI.Models
{
    public class Move
    {
        public int FromRow { get; set; }
        public int FromCol { get; set; }

        public int ToRow { get; set; }
        public int ToCol { get; set; }

        public char Piece { get; set; }

        public override string ToString()
        {
            return $"({FromRow},{FromCol}) -> ({ToRow},{ToCol})";
        }
    }
}