using ChessAI.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ChessAI.Views
{
    /// <summary>
    /// Interaction logic for Promote.xaml
    /// </summary>
    public partial class PromoteWindow : Window
    {
        public string ChosenPiece { get; set; } = "";
        public Turn _turn = Turn.Neither;
        public Visibility ShowBlack
        {
            get => (_turn == Turn.Black) ? Visibility.Visible : Visibility.Hidden;
        }
        public Visibility ShowWhite
        {
            get => (_turn == Turn.White) ? Visibility.Visible : Visibility.Hidden;
        }
        public PromoteWindow(Turn turn)
        {
            _turn = turn;
            InitializeComponent();
            DataContext = this;
        }

        private void ChoosePiece(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            ChosenPiece = button!.Name;
            if (!string.IsNullOrEmpty(ChosenPiece))
                this.Close();
        }
    }
}
