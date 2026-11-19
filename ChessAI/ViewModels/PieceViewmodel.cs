using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ChessAI.ViewModels
{
    public class PieceViewmodel : INotifyPropertyChanged
    {
        private static Dictionary<string, Dictionary<string, string>> localisation = new()
        {
            {
                "FI",
                new Dictionary<string, string>
                {
                    { "bishop", "lähetti" },
                    { "black", "musta" },
                    { "king", "kuningas" },
                    { "knight", "ratsu" },
                    { "pawn", "moukka" },
                    { "queen", "kuningatar" },
                    { "rook", "torni" },
                    { "white", "valkea" },
                }
            },
            {
                "SV",
                new Dictionary<string, string>
                {
                    { "bishop", "löpare" },
                    { "black", "svart" },
                    { "king", "kung" },
                    { "knight", "springare" },
                    { "pawn", "bonde" },
                    { "queen", "dam" },
                    { "rook", "torn" },
                    { "white", "vit" },
                }
           }
        };
        private int _row;
        private int _col;
        public string Name { get; set;  } = "";
        public bool OnBoard { get; set; } = false;
        public FrameworkElement? ScreenObj { get; set; }
        public Canvas? Container { get; set; }
        public bool IsWhite => Name.StartsWith("white");
        public char Id { get; set; }

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
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName=null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string HumanReadableName(string lang="FI")
        {
            string ret = "";
            Dictionary<string, string> dict = localisation[lang] ?? new Dictionary<string, string>();
            if (Name.StartsWith("white"))
                ret = dict.GetValueOrDefault("white", "white");
            else
                ret = dict.GetValueOrDefault("black", "black");
            ret += " ";
            string name = "";
            switch (Char.ToLower(Id))
            {
                case 'b':
                    name = "bishop";
                    break;
                case 'k':
                    name = "king";
                    break;
                case 'n':
                    name = "knight";
                    break;
                case 'p':
                    name = "pawn";
                    break;
                case 'q':
                    name = "queen";
                    break;
                case 'r':
                    name = "rook";
                    break;
                default:
                    name = $"{Id}";
                    break;
            }
            ret += dict.GetValueOrDefault(name, name);
            return ret;
        }
    }
}
