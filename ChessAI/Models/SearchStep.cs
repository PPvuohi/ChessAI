namespace ChessAI.Models
{
    public class SearchStep
    {
        // Hakupuun tämänhetkinen syvyys
        public int Depth { get; set; }

        // MAX tai MIN
        public string Player { get; set; } = "";

        // Tutkittava siirto, esimerkiksi "e2-e4"
        public string Move { get; set; } = "";

        // Peliaseman arviointifunktion antama arvo
        public int Score { get; set; }

        // Alpha-Beta-karsinnassa käytettävät arvot
        public int Alpha { get; set; }

        public int Beta { get; set; }

        // Karsittiinko tämä haara
        public bool IsPruned { get; set; }

        // Onko tämä paras tähän mennessä löydetty vaihtoehto
        public bool IsBestSoFar { get; set; }
    }
}
