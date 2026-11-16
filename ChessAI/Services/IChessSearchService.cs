using ChessAI.Models;

namespace ChessAI.Services
{
    public interface IChessSearchService
    {
        SearchResult FindBestMove(
            BoardState position,
            int maxDepth,
            bool useAlphaBeta);
    }
}