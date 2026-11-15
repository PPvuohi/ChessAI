using System.Collections.Generic;

namespace ChessAI.Models
{
    public class SearchResult
    {
        // Paras löydetty siirto
        public string BestMove { get; set; } = "";

        // Parhaan siirron arvio
        public int BestScore { get; set; }

        // Kuinka monta peliasemaa haussa tutkittiin
        public int NodesVisited { get; set; }

        // Haun aikana syntyneet vaiheet käyttöliittymää varten
        public List<SearchStep> Steps { get; set; } = new();
    }
}
