using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChessAI.Models;
using ChessAI.Services;

namespace ChessAI.ViewModels
{
    public enum Turn
    {
        White,
        Black,
        Neither
    }
    public class MainWindowViewmodel
    {
        public PieceViewmodel? Selected { get; set; } = null;
        public Turn CurrentTurn { get; set; } = Turn.White;
        public bool MayPromote
        {
            get
            {
                if (Selected == null)
                    return false;
                if (!(Selected.Name.Contains("Pawn") && Selected.OnBoard))
                    return false;
                if (Selected.IsWhite)
                    return Selected.Row == 8;
                return Selected.Row == 1;
            }
        }
    }
}
