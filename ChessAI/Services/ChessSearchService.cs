using ChessAI.Models;
using System.Collections.Generic;

namespace ChessAI.Services
{
    public class ChessSearchService : IChessSearchService
    {
        private readonly PositionEvaluator evaluator = new PositionEvaluator();
        private readonly MoveGenerator moveGenerator = new MoveGenerator();

        private int nodesVisited;

        // Pelin aikaisemmat asemat toistojen tarkistamista varten
        public IReadOnlyList<BoardState> PositionHistory { get; set; }
            = new List<BoardState>();

        public SearchResult FindBestMove(
            BoardState position,
            int maxDepth,
            bool useAlphaBeta)
        {
            nodesVisited = 0;

            List<SearchStep> steps = new List<SearchStep>();
            List<Move> moves = moveGenerator.GenerateMoves(position);

            string bestMove = "";
            Move? bestMoveData = null;

            int bestScore = position.WhiteToMove
                ? int.MinValue
                : int.MaxValue;

            int alpha = int.MinValue;
            int beta = int.MaxValue;

            // Muistetaan, johtaako paras siirto aiempaan asemaan.
            bool bestMoveRepeats = false;

            foreach (Move move in moves)
            {
                BoardState nextPosition = position.Clone();
                nextPosition.ApplyMove(move);

                int score;

                if (useAlphaBeta)
                {
                    score = AlphaBeta(
                        nextPosition,
                        maxDepth - 1,
                        maxDepth,
                        alpha,
                        beta,
                        steps);
                }
                else
                {
                    score = Minimax(
                        nextPosition,
                        maxDepth - 1,
                        maxDepth,
                        steps);
                }

                // Tarkistetaan, onko asema esiintynyt pelissä aiemmin.
                bool repeatsPosition = false;

                foreach (BoardState previousPosition in PositionHistory)
                {
                    if (IsSamePosition(nextPosition, previousPosition))
                    {
                        repeatsPosition = true;
                        break;
                    }
                }

                bool hasBetterScore = position.WhiteToMove
                ? score > bestScore
                : score < bestScore;

                // Tasatilanteessa suositaan siirtoa,
                // joka ei toista aiempaa asemaa.
                bool avoidsRepeatOnTie =
                    score == bestScore &&
                    bestMoveRepeats &&
                    !repeatsPosition;

                bool isBetter =
                    bestMoveData == null ||
                    hasBetterScore ||
                    avoidsRepeatOnTie;

                if (isBetter)
                {
                    bestScore = score;
                    bestMove = move.ToString();
                    bestMoveData = move;
                    bestMoveRepeats = repeatsPosition;
                }

                if (useAlphaBeta)
                {
                    if (position.WhiteToMove)
                        alpha = Math.Max(alpha, bestScore);
                    else
                        beta = Math.Min(beta, bestScore);
                }

                steps.Add(new SearchStep
                {
                    Depth = 1,
                    MaxDepth = maxDepth,
                    Side = position.WhiteToMove ? "White" : "Black",
                    Player = position.WhiteToMove ? "MAX" : "MIN",
                    Move = move.ToString(),
                    Score = score,
                    Alpha = alpha,
                    Beta = beta,
                    IsBestSoFar = isBetter
                });
            }

            return new SearchResult
            {
                BestMove = bestMove,
                BestMoveData = bestMoveData,
                BestScore = bestScore,
                NodesVisited = nodesVisited,
                Steps = steps
            };
        }

        private int Minimax(
            BoardState position,
            int depth,
            int maxDepth,
            List<SearchStep> steps)
        {
            nodesVisited++;

            if (depth <= 0)
                return evaluator.Evaluate(position);

            List<Move> moves = moveGenerator.GenerateMoves(position);

            if (moves.Count == 0)
                return evaluator.Evaluate(position);

            bool maximizing = position.WhiteToMove;

            int bestScore = maximizing
                ? int.MinValue
                : int.MaxValue;

            foreach (Move move in moves)
            {
                BoardState nextPosition = position.Clone();
                nextPosition.ApplyMove(move);

                int score = Minimax(
                    nextPosition,
                    depth - 1,
                    maxDepth,
                    steps);

                // MAX pyrkii maksimoimaan arvon
                if (maximizing)
                {
                    if (score > bestScore)
                        bestScore = score;
                }
                else
                {
                    // MIN pyrkii minimoimaan arvon
                    if (score < bestScore)
                        bestScore = score;
                }

                steps.Add(new SearchStep
                {
                    Depth = maxDepth - depth + 1,
                    MaxDepth = maxDepth,
                    Side = maximizing ? "White" : "Black",
                    Player = maximizing ? "MAX" : "MIN",
                    Move = move.ToString(),
                    Score = score
                });
            }

            return bestScore;
        }

        private int AlphaBeta(
            BoardState position,
            int depth,
            int maxDepth,
            int alpha,
            int beta,
            List<SearchStep> steps)
        {
            nodesVisited++;

            if (depth <= 0)
                return evaluator.Evaluate(position);

            List<Move> moves = moveGenerator.GenerateMoves(position);

            if (moves.Count == 0)
                return evaluator.Evaluate(position);

            bool maximizing = position.WhiteToMove;

            // MAX pyrkii maksimoimaan arvon
            if (maximizing)
            {
                int bestScore = int.MinValue;

                foreach (Move move in moves)
                {
                    BoardState nextPosition = position.Clone();
                    nextPosition.ApplyMove(move);

                    int score = AlphaBeta(
                        nextPosition,
                        depth - 1,
                        maxDepth,
                        alpha,
                        beta,
                        steps);

                    bestScore = Math.Max(bestScore, score);
                    alpha = Math.Max(alpha, bestScore);

                    bool pruned = beta <= alpha;

                    steps.Add(new SearchStep
                    {
                        Depth = maxDepth - depth + 1,
                        MaxDepth = maxDepth,
                        Side = "White",
                        Player = "MAX",
                        Move = move.ToString(),
                        Score = score,
                        Alpha = alpha,
                        Beta = beta,
                        IsPruned = pruned
                    });

                    // Lopetetaan haaran tutkiminen, jos beta <= alpha
                    if (pruned)
                        break;
                }

                return bestScore;
            }
            else
            {
                // MIN pyrkii minimoimaan arvon
                int bestScore = int.MaxValue;

                foreach (Move move in moves)
                {
                    BoardState nextPosition = position.Clone();
                    nextPosition.ApplyMove(move);

                    int score = AlphaBeta(
                        nextPosition,
                        depth - 1,
                        maxDepth,
                        alpha,
                        beta,
                        steps);

                    bestScore = Math.Min(bestScore, score);
                    beta = Math.Min(beta, bestScore);

                    bool pruned = beta <= alpha;

                    steps.Add(new SearchStep
                    {
                        Depth = maxDepth - depth + 1,
                        MaxDepth = maxDepth,
                        Side = "Black",
                        Player = "MIN",
                        Move = move.ToString(),
                        Score = score,
                        Alpha = alpha,
                        Beta = beta,
                        IsPruned = pruned
                    });

                    // Lopetetaan haaran tutkiminen, jos beta <= alpha
                    if (pruned)
                        break;
                }

                return bestScore;
            }

        }

        private static bool IsSamePosition(
             BoardState first,
             BoardState second)
        {
            // Tarkistetaan, kumman pelaajan vuoro on.
            if (first.WhiteToMove != second.WhiteToMove)
                return false;

            // Verrataan shakkilaudan kaikkia ruutuja.
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (first.Squares[row, col] !=
                        second.Squares[row, col])
                    {
                        return false;
                    }
                }
            }

            return true;

        }
    }
}