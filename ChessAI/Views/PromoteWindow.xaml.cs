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
        public PromoteWindow()
        {
            InitializeComponent();
        }

        private void ChoosePiece(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button!.Name == "ChooseKnight")
                ChosenPiece = "N";
            else if (button!.Name == "ChooseQueen")
                ChosenPiece = "Q";
            if (!string.IsNullOrEmpty(ChosenPiece))
                this.Close();
        }
    }
}
