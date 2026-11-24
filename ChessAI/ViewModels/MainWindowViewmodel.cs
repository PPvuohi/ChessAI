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
    public class MainWindowViewmodel : INotifyPropertyChanged
    {
        private PieceViewmodel? _selected = null;
        public PieceViewmodel? Selected {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MayPromote));
            }
        }
        public Turn CurrentTurn { get; set; } = Turn.White;
        public bool MayPromote
        {
            get
            {
                if (Selected == null)
                    return false;
                if (!(Char.ToLower(Selected.Id) == 'p' && Selected.OnBoard))
                    return false;
                if (Selected.IsWhite)
                    return Selected.Row == 8;
                return Selected.Row == 1;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
