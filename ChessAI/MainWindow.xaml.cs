using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChessAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void Difficulty_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            int squareSize = (int)cnvBoard.Width / 8;
            Brush? lightSquareBrush = (Brush?)Application.Current.FindResource("LightSquareBrush");
            Brush? darkSquareBrush = (Brush?)Application.Current.FindResource("DarkSquareBrush");
            if (lightSquareBrush == null || darkSquareBrush == null)
            {
                MessageBox.Show("LightSquareBrush or DarkSquareBrush resource not found.");
                return;
            }
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Rectangle square = new Rectangle
                    {
                        Width = squareSize,
                        Height = squareSize,
                        Fill = (row + col) % 2 == 0 ? lightSquareBrush : darkSquareBrush
                    };
                    Canvas.SetLeft(square, col * squareSize);
                    Canvas.SetTop(square, row * squareSize);
                    cnvBoard.Children.Add(square);
                }
            }
        }
    }
}