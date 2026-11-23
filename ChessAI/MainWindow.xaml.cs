using ChessAI.Models;
using ChessAI.Services;
using ChessAI.ViewModels;
using ChessAI.Views;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace ChessAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewmodel viewModel;
        private PieceViewmodel? selected
        {
            get => viewModel.Selected;
            set
            {
                viewModel.Selected = value;
            }
        }
        private ThinkingWindow thinkWindow;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new();
            DataContext = viewModel;
            thinkWindow = new ThinkingWindow(this);
            thinkWindow.Show();
        }

        private void Think_Click(object sender, RoutedEventArgs e)
        {
            if (GetCurrentTurn() == Turn.Neither)
            {
                MessageBox.Show("Valitse vuoroksi musta tai valkea.");
                return;
            }
            // Luetaan nykyinen pelitilanne käyttöliittymästä
            BoardState position = CreateBoardStateFromUI();

            int depth = 3;

            if (cmbDepth.SelectedItem is ComboBoxItem depthItem &&
                int.TryParse(depthItem.Content?.ToString(), out int selectedDepth))
            {
                depth = selectedDepth;
            }

            string searchMode = "Alpha-Beta";

            if (cmbSearchMode.SelectedItem is ComboBoxItem modeItem)
            {
                searchMode = modeItem.Content?.ToString() ?? "Alpha-Beta";
            }

            ClearMessages();

            // Tavallinen Minimax-haku
            if (searchMode == "Minimax")
            {
                ChessSearchService searchService = new ChessSearchService();

                SearchResult result = searchService.FindBestMove(
                    position,
                    depth,
                    false);

                if (result.BestMoveData == null)
                {
                    MessageBox.Show("Tekoäly ei löytänyt mahdollista siirtoa.");
                    return;
                }

                HighlightAiMove(result.BestMoveData);

                MessageBox.Show(
                    $"MINIMAX\n\n" +
                    $"Paras siirto: {result.BestMove}\n" +
                    $"Arvio: {result.BestScore}\n" +
                    $"Tutkitut asemat: {result.NodesVisited}");

                return;
            }

            // Minimax- ja Alpha-Beta-haun vertailu
            if (searchMode == "Vertailu")
            {
                ChessSearchService minimaxService = new ChessSearchService();
                ChessSearchService alphaBetaService = new ChessSearchService();

                SearchResult minimaxResult = minimaxService.FindBestMove(
                    position,
                    depth,
                    false);

                SearchResult alphaBetaResult = alphaBetaService.FindBestMove(
                    position,
                    depth,
                    true);

                double reduction = 0;

                if (minimaxResult.NodesVisited > 0)
                {
                    reduction =
                        (1.0 - (double)alphaBetaResult.NodesVisited /
                        minimaxResult.NodesVisited) * 100;
                }

                if (minimaxResult.BestMoveData == null ||
                    alphaBetaResult.BestMoveData == null)
                {
                    MessageBox.Show("Tekoäly ei löytänyt mahdollista siirtoa.");
                    return;
                }

                // Laudalla näytetään Alpha-Beta-haun valitsema siirto
                HighlightAiMove(alphaBetaResult.BestMoveData);

                MessageBox.Show(
                    $"MINIMAX\n" +
                    $"Paras siirto: {minimaxResult.BestMove}\n" +
                    $"Arvio: {minimaxResult.BestScore}\n" +
                    $"Tutkitut asemat: {minimaxResult.NodesVisited}\n\n" +

                    $"ALPHA-BETA\n" +
                    $"Paras siirto: {alphaBetaResult.BestMove}\n" +
                    $"Arvio: {alphaBetaResult.BestScore}\n" +
                    $"Tutkitut asemat: {alphaBetaResult.NodesVisited}" +
                    $"\n\nAlpha-Beta tutki {reduction:F1} % vähemmän asemia.");

                return;
            }

            // Oletuksena käytetään Alpha-Beta-karsintaa
            ChessSearchService alphaBetaSearchService = new ChessSearchService();

            SearchResult alphaBetaSearchResult = alphaBetaSearchService.FindBestMove(
                position,
                depth,
                true);

            if (alphaBetaSearchResult.BestMoveData == null)
            {
                MessageBox.Show("Tekoäly ei löytänyt mahdollista siirtoa.");
                return;
            }

            HighlightAiMove(alphaBetaSearchResult.BestMoveData);

            MessageBox.Show(
                $"ALPHA-BETA\n\n" +
                $"Paras siirto: {alphaBetaSearchResult.BestMove}\n" +
                $"Arvio: {alphaBetaSearchResult.BestScore}\n" +
                $"Tutkitut asemat: {alphaBetaSearchResult.NodesVisited}");
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            SetupBoard();
            ClearAll();
        }

        private void Promote_Click(object sender, RoutedEventArgs e)
        {
            if (selected == null)
                return;
            if (!viewModel.MayPromote)
                return;
            Image? piece = (Image?)selected.ScreenObj;
            if (piece == null)
                return;
            string newPiece = "Queen";
            PromoteWindow promote = new();
            promote.ShowDialog();
            switch (promote.ChosenPiece)
            {
                case "N":
                    newPiece = "Knight";
                    break;
                case "Q":
                    newPiece = "Queen";
                    break;
            }
            TransformInto(piece, newPiece, selected.IsWhite);
            selected.Id = GetPieceSymbol(piece);
            UpdateSelectedLabel();
        }
        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void Depth_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }
        private void cmbDepth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
                viewModel.CurrentTurn = GetCurrentTurn();
        }

        private void cnvPieces_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            lblSelected.Content = "";
            Turn currentTurn = GetCurrentTurn();
            string? captured = "";
            Point clickPos = e.GetPosition(cnvPieces);
            var (row, col) = FindSquare(clickPos, cnvPieces);
            FrameworkElement? piece = FindPieceAtPos(clickPos, cnvPieces);
            PieceViewmodel? targeted = null;
            if (piece != null)
                targeted = new()
                {
                    Name = piece.Name,
                    Col = col,
                    Row = row,
                    OnBoard = true,
                    ScreenObj = piece,
                    Id = GetPieceSymbol(piece),
                    Container = cnvPieces
                };
            if (selected != null && targeted != null)
            {
                if (selected.IsWhite != targeted.IsWhite)
                {
                    cnvPieces.Children.Remove(piece);
                    AddToTray(targeted.IsWhite ? cnvWhitePieces : cnvBlackPieces, targeted.ScreenObj!);
                    piece = null;
                    captured = targeted.HumanReadableName();
                    targeted = null;
                }

            }
            if (piece == null && selected != null)
            {
                bool fromTray = selected.Container != cnvPieces;
                if (fromTray)
                {
                    selected.Container?.Children.Remove(selected.ScreenObj);
                    cnvPieces.Children.Add(selected.ScreenObj);
                    selected.Container = cnvPieces;
                }
                FrameworkElement? toMove = FindPieceByName(selected.Name, cnvPieces);
                if (toMove != null)
                { 
                    PlacePiece(toMove.Name, row, col);
                    if (string.IsNullOrEmpty(captured))
                        lblSelected.Content = $"Kohde: {PositionName(row, col)}";
                    else
                        lblSelected.Content = $"Lyöty: {PositionName(row, col)} ({captured})";
                    ClearSelectedHighlights();
                    if (!fromTray && currentTurn != Turn.Neither && (selected.IsWhite == (currentTurn == Turn.White)))
                        AddMoveToHistory(GetCurrentTurn(), new Move()
                        {
                            FromRow = selected.Row,
                            FromCol = selected.Col,
                            ToRow = row,
                            ToCol = col,
                            Piece = selected.Id,
                            Capture = !string.IsNullOrEmpty(captured)
                        });
                    YieldTurn(selected);
                    selected = null;
                }
            }
            else if (targeted != null)
            {
                // toistaiseksi aina sallittu koska vuorojen valvonta hankaloittaa tiettyjä juttuja
                if (true || currentTurn == Turn.Neither || currentTurn == (targeted.IsWhite ? Turn.White : Turn.Black))
                {
                    selected = targeted;
                    AddSelectedHighlight(row, col, cnvBoard);
                    UpdateSelectedLabel();
                }
            }
        }

        private void UpdateSelectedLabel()
        {
            if (selected == null)
                return;
            lblSelected.Content = $"Valittu: {PositionName(selected.Row, selected.Col)} ({selected.HumanReadableName()})";
            if (viewModel.MayPromote)
                lblSelected.Content += "\nVoi korottaa";
        }

        private void cnvWhitePieces_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TrayInteraction(cnvWhitePieces, e.GetPosition(cnvWhitePieces));
        }
        private void cnvBlackPieces_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TrayInteraction(cnvBlackPieces, e.GetPosition(cnvBlackPieces));
        }
        private void TrayInteraction(Canvas tray, Point clickPos)
        {
            lblSelected.Content = "";
            FrameworkElement? piece = FindPieceAtPos(clickPos, tray);
            if (piece == null && selected != null)
            {
                if (selected.Container == tray || selected.ScreenObj == null)
                    return;
                selected.Container?.Children.Remove(selected.ScreenObj);
                Canvas targetContainer;
                if (selected.Name.StartsWith("black"))
                    targetContainer = cnvBlackPieces;
                else
                    targetContainer = cnvWhitePieces;
                AddToTray(targetContainer, selected.ScreenObj);
                selected.Container = targetContainer;
                ClearSelectedHighlights();
                selected = null;
            }
            else if (piece != null)
            {
                selected = new ViewModels.PieceViewmodel()
                {
                    Name = piece.Name,
                    OnBoard = false,
                    ScreenObj = piece,
                    Id = GetPieceSymbol(piece),
                    Container = tray
                };
                var (row, col) = FindSquare(clickPos, tray);
                AddSelectedHighlight(row, col, (tray == cnvWhitePieces ? cnvWhiteBoard : cnvBlackBoard));
                lblSelected.Content = $"Valittu: {selected.HumanReadableName()}";
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // piirrä taustaruudukko
            int squareSize = (int)cnvBoard.Width / 8;
            Brush? lightSquareBrush = (Brush?)Application.Current.FindResource("LightSquareBrush");
            Brush? darkSquareBrush = (Brush?)Application.Current.FindResource("DarkSquareBrush");
            if (lightSquareBrush == null || darkSquareBrush == null)
            {
                MessageBox.Show("Taustaväriä LightSquareBrush ja/tai DarkSquareBrush ei ole määritelty.");
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
            SetupBoard();
        }
        private void FromTrayToBoard(Canvas tray)
        {
            List<FrameworkElement> pieces = [.. tray.Children.Cast<FrameworkElement>()];
            foreach (FrameworkElement piece in pieces)
            {
                tray.Children.Remove(piece);
                cnvPieces.Children.Add(piece);
            }
        }
        private void SetupBoard()
        {
            FromTrayToBoard(cnvWhitePieces);
            FromTrayToBoard(cnvBlackPieces);
            // asettele nappulat aloituspaikoilleen
            int squareSize = (int)cnvBoard.Width / 8;
            double squareCenter = squareSize / 2.0;
            for (int col = 1; col <= 8; col++)
            {
                // valkoiset sotilaat
                PlacePiece($"whitePawn{col}", 2, col);
                // mustat sotilaat
                PlacePiece($"blackPawn{col}", 7, col);
            }
            // perutaan promootiot
            List<Image> pawns = cnvPieces.Children
                                    .OfType<Image>()
                                    .Where(r => r.Name.Contains("Pawn"))
                                    .ToList();
            foreach (Image pawn in pawns)
                if (!Equals(pawn.Tag, "Pawn"))
                    TransformInto(pawn, "Pawn", pawn.Name.StartsWith("white"));
            // keksitään tähän ehkä parempi rutiini myöhemmin
            PlacePiece("whiteRook1", 1, 1);
            PlacePiece("whiteKnight1", 1, 2);
            PlacePiece("whiteBishop1", 1, 3);
            PlacePiece("whiteQueen", 1, 4);
            PlacePiece("whiteKing", 1, 5);
            PlacePiece("whiteBishop2", 1, 6);
            PlacePiece("whiteKnight2", 1, 7);
            PlacePiece("whiteRook2", 1, 8);
            PlacePiece("blackRook1", 8, 1);
            PlacePiece("blackKnight1", 8, 2);
            PlacePiece("blackBishop1", 8, 3);
            PlacePiece("blackQueen", 8, 4);
            PlacePiece("blackKing", 8, 5);
            PlacePiece("blackBishop2", 8, 6);
            PlacePiece("blackKnight2", 8, 7);
            PlacePiece("blackRook2", 8, 8);
        }
        private void PlacePiece(string pieceName, int row, int col)
        {
            double boardWidth = cnvBoard.Width;
            double squareSize = boardWidth / 8;
            double halfSquare = squareSize / 2.0;
            FrameworkElement? piece = FindPieceByName(pieceName, cnvPieces);
            if (piece == null)
                return;
            double halfWidth = piece.Width / 2.0;
            double halfHeight = piece.Height / 2.0;
            // oletetaan että rivi 1 on alareunassa
            Canvas.SetLeft(piece, (col - 1) * squareSize + (halfSquare - halfWidth));
            Canvas.SetTop(piece, boardWidth - row * squareSize + (halfSquare - halfHeight));
            piece.Visibility = Visibility.Visible;
        }
        private FrameworkElement? FindPieceByName(string name, Canvas container)
        {
            foreach (FrameworkElement piece in container.Children)
                if (piece.Name == name)
                    return piece;
            return null;
        }
        private FrameworkElement? FindPieceAtPos(Point pos, Canvas container)
        {
            double x = pos.X;
            double y = pos.Y;
            foreach (FrameworkElement piece in container.Children)
            {
                double left = Canvas.GetLeft(piece);
                double right = Canvas.GetRight(piece);
                double top = Canvas.GetTop(piece);
                double bottom = Canvas.GetBottom(piece);
                double width = piece.Width;
                double height = piece.Height;
                string name = piece.Name;
                if (
                    left <= x &&
                    left + width >= x &&
                    top <= y &&
                    top + height >= y
                    )
                    return piece;
            }
            return null;
        }
        private (int, int) FindSquare(Point pos, Canvas container)
        {
            double boardHeight = container.Height;
            double squareSize = boardHeight / 8;
            double x = pos.X;
            double y = pos.Y;
            int row = 8 - (int)(y / squareSize);  // rivi 1 on alareunassa
            int col = (int)(x / squareSize) + 1;
            return (row, col);
        }
        private void ClearAll()
        {
            // Tyhjennetään siirtohistoria
            txbMovesWhite.Text = "";
            txbMovesBlack.Text = "";

            // pyyhitään muut jutut
            selected = null;
            ClearSelectedHighlights();
            ClearAIHighlights();
            ClearMessages();
        }
        private void AddSelectedHighlight(int row, int col, Canvas container)
        {
            ClearSelectedHighlights();
            Style highlightStyle = (Style)FindResource("SelectedHighlightStyle");
            double boardHeight = container.Height;
            double squareSize = boardHeight / 8;
            Rectangle highlight = new()
            {
                Width = squareSize,
                Height = squareSize,
                Style = highlightStyle,
                Tag = "SelectedHighlight"
            };
            Canvas.SetLeft(highlight, ((col - 1) * squareSize));
            Canvas.SetTop(highlight, ((8 - row) * squareSize));
            container.Children.Add(highlight);
        }
        private void ClearSelectedHighlights()
        {
            ClearSelectedHighlight(cnvBoard);
            ClearSelectedHighlight(cnvBlackBoard);
            ClearSelectedHighlight(cnvWhiteBoard);
        }
        private void ClearAIHighlights()
        {
            List<Rectangle> oldHighlights =
                cnvBoard.Children
                    .OfType<Rectangle>()
                    .Where(r => Equals(r.Tag, "AiHighlight"))
                    .ToList();

            foreach (Rectangle highlight in oldHighlights)
            {
                cnvBoard.Children.Remove(highlight);
            }
        }
        private void ClearSelectedHighlight(Canvas container)
        {
            List<Rectangle> highlights = container.Children
                                            .OfType<Rectangle>()
                                            .Where(r => Equals(r.Tag, "SelectedHighlight"))
                                            .ToList();
            foreach (Rectangle h in highlights)
                container.Children.Remove(h);
        }
        private string PositionName(int row, int col)
        {
            if (row < 1 || col < 1 || row > 8 || col > 8)
                return "mahoton";
            return $"{(char)((int)'a' + col - 1)}{row}";
        }
        private void ClearMessages()
        {
            lblSelected.Content = "";
        }
        private void AddMoveToHistory(Turn turn, Move move)
        {
            string moveString = $"{(Char.ToLower(move.Piece) == 'p' ? "" : Char.ToUpper(move.Piece))}";
            if (move.Capture)
                moveString += "x";
            moveString += $"{PositionName(move.FromRow, move.FromCol)}-{PositionName(move.ToRow, move.ToCol)}";
            if (turn == Turn.White)
                txbMovesWhite.Text += moveString + "\n";
            else if (turn == Turn.Black)
                txbMovesBlack.Text += moveString + "\n";
        }

        private void YieldTurn(PieceViewmodel piece)
        {
            Turn turn = GetCurrentTurn();
            if (turn == Turn.Neither)
                return;

            bool isWhite = piece.IsWhite;
            if (isWhite && turn == Turn.White)
                SetTurn(Turn.Black);
            else if (!isWhite && turn == Turn.Black)
                SetTurn(Turn.White);
        }
        private void SetTurn(Turn turn)
        {
            if (turn == Turn.White)
            {
                cmbActivePlayer.SelectedItem = cmbItemWhite;
            }
            else if (turn == Turn.Black)
            {
                cmbActivePlayer.SelectedItem = cmbItemBlack;
            }
        }
        private void ClearBoard_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
            List<FrameworkElement> toRemove = [.. cnvPieces.Children.Cast<FrameworkElement>()];
            foreach (FrameworkElement piece in toRemove)
            {
                cnvPieces.Children.Remove(piece);
                if (piece.Name.StartsWith("white"))
                {
                    AddToTray(cnvWhitePieces, piece);
                }
                else if (piece.Name.StartsWith("black"))
                {
                    AddToTray(cnvBlackPieces, piece);
                }
            }
        }
        private void AddToTray(Canvas tray, FrameworkElement piece)
        {
            if (tray.Children.Contains(piece))
                return;
            tray.Children.Add(piece);
            double trayWidth = tray.Width;
            double squareSize = trayWidth / 2;
            double halfSquare = squareSize / 2.0;
            double halfWidth = piece.Width / 2.0;
            double halfHeight = piece.Height / 2.0;
            FrameworkElement? occupant;
            Point pos;
            for (int col = 0; col <= 1; col++)
                for (int row = 0; row <= 7; row++)
                {
                    pos = new(col * squareSize + halfSquare, row * squareSize + halfSquare);
                    occupant = FindPieceAtPos(pos, tray);
                    if (occupant == null)   // löytyi tyhjä paikka
                    {
                        Canvas.SetLeft(piece, col * squareSize + (halfSquare - halfWidth));
                        Canvas.SetTop(piece, row * squareSize + (halfSquare - halfWidth));
                        piece.Visibility = Visibility.Visible;
                        return;
                    }
                }
        }

        private void cmbActivePlayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        // Muodostetaan käyttöliittymän nappuloista pelitilanne tekoälylle
        private BoardState CreateBoardStateFromUI()
        {
            BoardState position = new BoardState();

            double squareSize = cnvBoard.Width / 8;

            foreach (FrameworkElement piece in cnvPieces.Children)
            {
                double left = Canvas.GetLeft(piece);
                double top = Canvas.GetTop(piece);

                if (double.IsNaN(left) || double.IsNaN(top))
                    continue;

                int col = (int)((left + piece.Width / 2) / squareSize);
                int row = (int)((top + piece.Height / 2) / squareSize);

                if (row < 0 || row >= 8 || col < 0 || col >= 8)
                    continue;

                char symbol = GetPieceSymbol(piece);

                if (symbol != '.')
                {
                    position.Squares[row, col] = symbol;
                }
            }

            position.WhiteToMove = GetCurrentTurn() != Turn.Black;

            return position;
        }

        public Turn GetCurrentTurn()
        {
            if (cmbActivePlayer.SelectedItem is ComboBoxItem playerItem)
            {
                if (playerItem == cmbItemBlack)
                    return Turn.Black;
                else if (playerItem == cmbItemWhite)
                    return Turn.White;
                else
                    return Turn.Neither;
            }
            return Turn.Neither;
        }
        // Määritellään nappulaa vastaava tekoälyn käyttämä merkki
        private char GetPieceSymbol(FrameworkElement piece)
        {
            string name = piece.Name;
            bool white =
                name.StartsWith(
                    "white",
                    StringComparison.OrdinalIgnoreCase);

            char pieceSymbol;

            if (Equals(piece.Tag, "Pawn"))
                pieceSymbol = 'P';
            else if (Equals(piece.Tag, "Knight"))
                pieceSymbol = 'N';
            else if (Equals(piece.Tag, "Bishop"))
                pieceSymbol = 'B';
            else if (Equals(piece.Tag, "Rook"))
                pieceSymbol = 'R';
            else if (Equals(piece.Tag, "Queen"))
                pieceSymbol = 'Q';
            else if (Equals(piece.Tag, "King"))
                pieceSymbol = 'K';
            else
                return '.';

            return white
                ? pieceSymbol
                : char.ToLowerInvariant(pieceSymbol);
        }
        private void TransformInto(Image piece, string piecetype, bool white)
        {
            string uri = $"/Graphics/{(white ? "white" : "black")}_{piecetype.ToLower()}.png";
            piece.Source = new BitmapImage(new Uri(uri, UriKind.Relative));
            piece.Tag = piecetype;
        }

        // Korostetaan tekoälyn valitseman siirron lähtö- ja kohderuutu
        private void HighlightAiMove(Move move)
        {
            ClearAIHighlights();

            double squareSize = cnvBoard.Width / 8;

            AddAiHighlight(
                move.FromRow,
                move.FromCol,
                squareSize,
                Brushes.DeepSkyBlue);

            AddAiHighlight(
                move.ToRow,
                move.ToCol,
                squareSize,
                Brushes.Gold);
        }

        // Korostetaan tekoälyn valitseman siirron ruutu laudalla
        private void AddAiHighlight(
             int row,
             int col,
             double squareSize,
             Brush color)
        {
            Rectangle highlight = new Rectangle
            {
                Width = squareSize,
                Height = squareSize,
                Stroke = color,
                StrokeThickness = 5,
                Fill = Brushes.Transparent,
                Tag = "AiHighlight"
            };

            // Asetetaan korostus oikean ruudun kohdalle
            Canvas.SetLeft(highlight, col * squareSize);
            Canvas.SetTop(highlight, row * squareSize);

            cnvBoard.Children.Add(highlight);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            thinkWindow?.Close();
        }

    }

}