using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChessAI.ViewModels
{
    public class PieceViewmodel : INotifyPropertyChanged
    {
        private int _row;
        private int _col;
        public string Name { get; } = "";
        
        public int Row
        {
            get => _row;
            set {
                _row = value;
                OnPropertyChanged();
            }
        }
        public int Col
        {
            get => _col;
            set
            {
                _col = value;
                OnPropertyChanged();
            }
        }
        public PieceViewmodel(string name)
        {
            Name = name;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName=null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
